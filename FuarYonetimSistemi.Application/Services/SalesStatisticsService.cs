using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Domain.Entities;
using FuarYonetimSistemi.Domain.Enums;
using FuarYonetimSistemi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Services
{
    public class SalesStatisticsService : ISalesStatisticsService
    {
        private readonly AppDbContext _context;

        public SalesStatisticsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SalesStatisticsDto> GetUserSalesStatisticsAsync(Guid userId, SalesStatisticsRequestDto request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new ArgumentException("Kullanıcı bulunamadı");

            var query = _context.Stands
                .Include(s => s.Payments)
                .Include(s => s.Fair)
                .Where(s => s.SalesRepresentativeId == userId);

            // Tarih filtreleri
            if (request.StartDate.HasValue)
                query = query.Where(s => s.ContractDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                query = query.Where(s => s.ContractDate <= request.EndDate.Value);

            if (request.FairId.HasValue)
                query = query.Where(s => s.FairId == request.FairId.Value);

            var stands = await query.ToListAsync();
            var currentDate = DateTime.Now;

            var statistics = new SalesStatisticsDto
            {
                UserId = userId,
                UserName = user.FullName,
                UserEmail = user.Email,
                UserRole = user.Role.ToString(),

                TotalStandsSold = stands.Count,
                TotalRevenue = stands.Sum(s => s.SaleAmountWithoutVAT ?? 0),
                TotalWithVAT = stands.Sum(s => s.TotalAmountWithVAT ?? 0),
                TotalPaymentsReceived = stands.SelectMany(s => s.Payments).Sum(p => p.Amount),
                TotalOutstandingBalance = stands.Sum(s => s.Balance ?? 0),

                AverageStandValue = stands.Count > 0 ? stands.Average(s => s.TotalAmountWithVAT ?? 0) : 0,
                AverageStandArea = stands.Count > 0 ? stands.Average(s => s.ContractArea ?? 0) : 0,

                StandsSoldThisMonth = stands.Count(s => s.ContractDate.HasValue &&
                    s.ContractDate.Value.Year == currentDate.Year &&
                    s.ContractDate.Value.Month == currentDate.Month),

                StandsSoldThisYear = stands.Count(s => s.ContractDate.HasValue &&
                    s.ContractDate.Value.Year == currentDate.Year),

                RevenueThisMonth = stands
                    .Where(s => s.ContractDate.HasValue &&
                               s.ContractDate.Value.Year == currentDate.Year &&
                               s.ContractDate.Value.Month == currentDate.Month)
                    .Sum(s => s.TotalAmountWithVAT ?? 0),

                RevenueThisYear = stands
                    .Where(s => s.ContractDate.HasValue &&
                               s.ContractDate.Value.Year == currentDate.Year)
                    .Sum(s => s.TotalAmountWithVAT ?? 0),

                OverdueStands = stands.Count(s => s.ActualDueDate.HasValue &&
                    s.ActualDueDate.Value < currentDate && (s.Balance ?? 0) > 0),

                StandsDueSoon = stands.Count(s => s.ActualDueDate.HasValue &&
                    s.ActualDueDate.Value >= currentDate &&
                    s.ActualDueDate.Value <= currentDate.AddDays(30) && (s.Balance ?? 0) > 0),

                OverdueAmount = stands
                    .Where(s => s.ActualDueDate.HasValue &&
                               s.ActualDueDate.Value < currentDate && (s.Balance ?? 0) > 0)
                    .Sum(s => s.Balance ?? 0)
            };

            // Tahsilat oranını hesapla
            var totalExpected = statistics.TotalWithVAT;
            statistics.PaymentCollectionRate = totalExpected > 0
                ? (statistics.TotalPaymentsReceived / totalExpected) * 100
                : 0;

            return statistics;
        }

        public async Task<TeamSalesStatisticsDto> GetTeamSalesStatisticsAsync(Guid managerId, SalesStatisticsRequestDto request)
        {
            var manager = await _context.Users.FindAsync(managerId);
            if (manager == null) throw new ArgumentException("Yönetici bulunamadı");

            var teamMembers = await GetManagedUsersAsync(managerId);
            var teamMemberIds = teamMembers.Select(u => u.Id).ToList();

            var teamStatistics = new List<SalesStatisticsDto>();

            foreach (var member in teamMembers)
            {
                var memberStats = await GetUserSalesStatisticsAsync(member.Id, request);
                teamStatistics.Add(memberStats);
            }

            var topPerformer = teamStatistics
                .OrderByDescending(s => s.TotalRevenue)
                .FirstOrDefault();

            return new TeamSalesStatisticsDto
            {
                ManagerId = managerId,
                ManagerName = manager.FullName,
                TotalTeamMembers = teamMembers.Count(),
                TotalTeamStandsSold = teamStatistics.Sum(s => s.TotalStandsSold),
                TotalTeamRevenue = teamStatistics.Sum(s => s.TotalRevenue),
                TotalTeamPayments = teamStatistics.Sum(s => s.TotalPaymentsReceived),
                TeamPaymentCollectionRate = teamStatistics.Count > 0
                    ? teamStatistics.Average(s => s.PaymentCollectionRate)
                    : 0,
                TopPerformerName = topPerformer?.UserName ?? "",
                TopPerformerStands = topPerformer?.TotalStandsSold ?? 0,
                TopPerformerRevenue = topPerformer?.TotalRevenue ?? 0,
                TeamMemberStatistics = teamStatistics
            };
        }

        public async Task<DashboardStatisticsDto> GetDashboardStatisticsAsync(Guid currentUserId, SalesStatisticsRequestDto request)
        {
            var currentUser = await _context.Users.FindAsync(currentUserId);
            if (currentUser == null) throw new ArgumentException("Kullanıcı bulunamadı");

            // Kullanıcının erişim yetkisi olan standları al
            var accessibleStands = await GetAccessibleStandsAsync(currentUserId);
            var currentDate = DateTime.Now;

            // Temel istatistikler
            var totalRevenue = accessibleStands.Sum(s => s.TotalAmountWithVAT ?? 0);
            var totalPayments = accessibleStands.SelectMany(s => s.Payments).Sum(p => p.Amount);

            // Top performans
            var topSalesReps = await _context.Stands
                .Where(s => accessibleStands.Select(a => a.Id).Contains(s.Id))
                .GroupBy(s => s.SalesRepresentativeId)
                .Select(g => new TopPerformerDto
                {
                    UserId = g.Key ?? Guid.Empty,
                    StandsSold = g.Count(),
                    Revenue = g.Sum(s => s.TotalAmountWithVAT ?? 0)
                })
                .OrderByDescending(t => t.Revenue)
                .Take(5)
                .ToListAsync();

            // User isimlerini al
            foreach (var performer in topSalesReps)
            {
                var user = await _context.Users.FindAsync(performer.UserId);
                performer.UserName = user?.FullName ?? "Bilinmeyen";
            }

            // Fair performansları
            var topFairs = await _context.Stands
                .Include(s => s.Fair)
                .Where(s => accessibleStands.Select(a => a.Id).Contains(s.Id))
                .GroupBy(s => new { s.FairId, s.Fair.Name })
                .Select(g => new FairPerformanceDto
                {
                    FairId = g.Key.FairId ?? Guid.Empty,
                    FairName = g.Key.Name ?? "",
                    StandsSold = g.Count(),
                    Revenue = g.Sum(s => s.TotalAmountWithVAT ?? 0),
                    ParticipantCount = g.Select(s => s.ParticipantId).Distinct().Count()
                })
                .OrderByDescending(f => f.Revenue)
                .Take(5)
                .ToListAsync();

            return new DashboardStatisticsDto
            {
                TotalActiveStands = accessibleStands.Count(),
                TotalSystemRevenue = totalRevenue,
                TotalOutstandingBalance = accessibleStands.Sum(s => s.Balance ?? 0),
                TotalActiveSalesReps = accessibleStands
                    .Where(s => s.SalesRepresentativeId.HasValue)
                    .Select(s => s.SalesRepresentativeId.Value)
                    .Distinct()
                    .Count(),

                StandsThisMonth = accessibleStands.Count(s => s.ContractDate.HasValue &&
                    s.ContractDate.Value.Year == currentDate.Year &&
                    s.ContractDate.Value.Month == currentDate.Month),

                StandsLastMonth = accessibleStands.Count(s => s.ContractDate.HasValue &&
                    s.ContractDate.Value.Year == currentDate.Year &&
                    s.ContractDate.Value.Month == currentDate.Month - 1),

                RevenueThisMonth = accessibleStands
                    .Where(s => s.ContractDate.HasValue &&
                               s.ContractDate.Value.Year == currentDate.Year &&
                               s.ContractDate.Value.Month == currentDate.Month)
                    .Sum(s => s.TotalAmountWithVAT ?? 0),

                RevenueLastMonth = accessibleStands
                    .Where(s => s.ContractDate.HasValue &&
                               s.ContractDate.Value.Year == currentDate.Year &&
                               s.ContractDate.Value.Month == currentDate.Month - 1)
                    .Sum(s => s.TotalAmountWithVAT ?? 0),

                AverageStandValue = accessibleStands.Where(s => s.TotalAmountWithVAT.HasValue).Any()
    ? accessibleStands.Where(s => s.TotalAmountWithVAT.HasValue).Average(s => s.TotalAmountWithVAT.Value)
    : 0,

                SystemPaymentCollectionRate = totalRevenue > 0
                    ? (totalPayments / totalRevenue) * 100
                    : 0,

                OverdueStandsCount = accessibleStands.Count(s => s.ActualDueDate.HasValue &&
                    s.ActualDueDate.Value < currentDate && (s.Balance ?? 0) > 0),

                StandsDueSoonCount = accessibleStands.Count(s => s.ActualDueDate.HasValue &&
                    s.ActualDueDate.Value >= currentDate &&
                    s.ActualDueDate.Value <= currentDate.AddDays(30) && (s.Balance ?? 0) > 0),

                OverdueAmount = accessibleStands
                    .Where(s => s.ActualDueDate.HasValue &&
                               s.ActualDueDate.Value < currentDate && (s.Balance ?? 0) > 0)
                    .Sum(s => s.Balance ?? 0),

                TopSalesReps = topSalesReps,
                TopFairs = topFairs
            };
        }

        public async Task<IEnumerable<Stand>> GetAccessibleStandsAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return new List<Stand>();

            var query = _context.Stands
                .Include(s => s.Payments)
                .Include(s => s.SalesRepresentative)
                .Include(s => s.Fair)
                .Include(s => s.Participant)
                .AsQueryable();

            switch (user.Role)
            {
                case UserRole.Admin:
                    // Admin tüm standları görebilir
                    return await query.ToListAsync();

                case UserRole.Manager:
                    // Manager kendi sattıklarını + alt kademe çalışanlarının sattıklarını görebilir
                    var managedUsers = await GetManagedUsersAsync(userId);
                    var managedUserIds = managedUsers.Select(u => u.Id).ToList();
                    managedUserIds.Add(userId); // Kendi satışlarını da dahil et

                    return await query
                        .Where(s => s.SalesRepresentativeId.HasValue &&
                                   managedUserIds.Contains(s.SalesRepresentativeId.Value))
                        .ToListAsync();

                case UserRole.SalesPerson:
                    // SalesPerson sadece kendi sattıklarını görebilir
                    return await query
                        .Where(s => s.SalesRepresentativeId == userId)
                        .ToListAsync();

                default:
                    return new List<Stand>();
            }
        }

        public async Task<IEnumerable<User>> GetManagedUsersAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return new List<User>();

            switch (user.Role)
            {
                case UserRole.Admin:
                    // Admin tüm kullanıcıları yönetebilir
                    return await _context.Users
                        .Where(u => !u.IsDeleted && u.Id != userId)
                        .ToListAsync();

                case UserRole.Manager:
                    // Manager SalesPerson'ları yönetebilir
                    return await _context.Users
                        .Where(u => !u.IsDeleted && u.Role == UserRole.SalesPerson)
                        .ToListAsync();

                default:
                    return new List<User>();
            }
        }
    }
}
