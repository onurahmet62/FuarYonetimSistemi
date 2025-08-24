using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Application.Services;
using FuarYonetimSistemi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager,SalesPerson")]
    public class SalesStatisticsController : ControllerBase
    {
        private readonly ISalesStatisticsService _salesStatisticsService;

        public SalesStatisticsController(ISalesStatisticsService salesStatisticsService)
        {
            _salesStatisticsService = salesStatisticsService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı");
            }
            return userId;
        }

        /// <summary>
        /// Kullanıcının yönetebileceği kullanıcı listesini getirir
        /// </summary>
        [HttpGet("managed-users")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> GetManagedUsers()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var users = await _salesStatisticsService.GetManagedUsersAsync(currentUserId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Satış performans raporu - Excel export için
        /// </summary>
        [HttpPost("performance-report")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> GetPerformanceReport([FromBody] SalesStatisticsRequestDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var managedUsers = await _salesStatisticsService.GetManagedUsersAsync(currentUserId);

                var reports = new List<object>();

                foreach (var user in managedUsers)
                {
                    var stats = await _salesStatisticsService.GetUserSalesStatisticsAsync(user.Id, request);
                    reports.Add(new
                    {
                        UserName = stats.UserName,
                        UserEmail = stats.UserEmail,
                        Role = stats.UserRole,
                        TotalStands = stats.TotalStandsSold,
                        TotalRevenue = stats.TotalRevenue,
                        PaymentCollectionRate = $"{stats.PaymentCollectionRate:F1}%",
                        AverageStandValue = stats.AverageStandValue,
                        StandsThisMonth = stats.StandsSoldThisMonth,
                        RevenueThisMonth = stats.RevenueThisMonth,
                        OverdueStands = stats.OverdueStands,
                        OverdueAmount = stats.OverdueAmount
                    });
                }

                return Ok(reports);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Vadesi yaklaşan veya geçen standları getirir
        /// </summary>
        [HttpGet("due-alerts")]
        public async Task<ActionResult> GetDueAlerts()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var accessibleStands = await _salesStatisticsService.GetAccessibleStandsAsync(currentUserId);
                var currentDate = DateTime.Now;

                var overdueStands = accessibleStands
                    .Where(s => s.ActualDueDate.HasValue &&
                               s.ActualDueDate.Value < currentDate &&
                               (s.Balance ?? 0) > 0)
                    .Select(s => new
                    {
                        s.Id,
                        s.Name,
                        s.FairHall,
                        ParticipantName = s.Participant?.CompanyName ?? "",
                        FairName = s.Fair?.Name ?? "",
                        SalesRepName = s.SalesRepresentative?.FullName ?? "",
                        DueDate = s.ActualDueDate,
                        DaysOverdue = (int)(currentDate - s.ActualDueDate.Value).TotalDays,
                        OutstandingAmount = s.Balance ?? 0
                    })
                    .OrderBy(s => s.DueDate)
                    .ToList();

                var dueSoonStands = accessibleStands
                    .Where(s => s.ActualDueDate.HasValue &&
                               s.ActualDueDate.Value >= currentDate &&
                               s.ActualDueDate.Value <= currentDate.AddDays(30) &&
                               (s.Balance ?? 0) > 0)
                    .Select(s => new
                    {
                        s.Id,
                        s.Name,
                        s.FairHall,
                        ParticipantName = s.Participant?.CompanyName ?? "",
                        FairName = s.Fair?.Name ?? "",
                        SalesRepName = s.SalesRepresentative?.FullName ?? "",
                        DueDate = s.ActualDueDate,
                        DaysUntilDue = (int)(s.ActualDueDate.Value - currentDate).TotalDays,
                        OutstandingAmount = s.Balance ?? 0
                    })
                    .OrderBy(s => s.DueDate)
                    .ToList();

                return Ok(new
                {
                    OverdueStands = overdueStands,
                    DueSoonStands = dueSoonStands,
                    TotalOverdueAmount = overdueStands.Sum(s => s.OutstandingAmount),
                    TotalDueSoonAmount = dueSoonStands.Sum(s => s.OutstandingAmount)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Aylık karşılaştırmalı rapor
        /// </summary>
        [HttpGet("monthly-comparison")]
        public async Task<ActionResult> GetMonthlyComparison([FromQuery] int year = 0)
        {
            try
            {
                if (year == 0) year = DateTime.Now.Year;

                var currentUserId = GetCurrentUserId();
                var accessibleStands = await _salesStatisticsService.GetAccessibleStandsAsync(currentUserId);

                var monthlyData = new List<object>();

                for (int month = 1; month <= 12; month++)
                {
                    var monthStands = accessibleStands
                        .Where(s => s.ContractDate.HasValue &&
                                   s.ContractDate.Value.Year == year &&
                                   s.ContractDate.Value.Month == month)
                        .ToList();

                    monthlyData.Add(new
                    {
                        Month = month,
                        MonthName = new DateTime(year, month, 1).ToString("MMMM"),
                        StandsSold = monthStands.Count,
                        Revenue = monthStands.Sum(s => s.TotalAmountWithVAT ?? 0),
                        PaymentsReceived = monthStands.SelectMany(s => s.Payments)
                            .Where(p => p.PaymentDate.Year == year && p.PaymentDate.Month == month)
                            .Sum(p => p.Amount),
                        UniqueCustomers = monthStands.Select(s => s.ParticipantId).Distinct().Count(),
                        AverageStandValue = monthStands.Count > 0 ? monthStands.Average(s => s.TotalAmountWithVAT ?? 0) : 0
                    });
                }

                return Ok(new
                {
                    Year = year,
                    MonthlyData = monthlyData,
                    YearTotal = new
                    {
                        TotalStands = accessibleStands.Count(s => s.ContractDate.HasValue && s.ContractDate.Value.Year == year),
                        TotalRevenue = accessibleStands
                            .Where(s => s.ContractDate.HasValue && s.ContractDate.Value.Year == year)
                            .Sum(s => s.TotalAmountWithVAT ?? 0),
                        TotalPayments = accessibleStands
                            .SelectMany(s => s.Payments)
                            .Where(p => p.PaymentDate.Year == year)
                            .Sum(p => p.Amount)
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// </summary>
        [HttpPost("my-statistics")]
        public async Task<ActionResult<SalesStatisticsDto>> GetMyStatistics([FromBody] SalesStatisticsRequestDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var statistics = await _salesStatisticsService.GetUserSalesStatisticsAsync(userId, request);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Belirli bir kullanıcının satış istatistiklerini getirir (Sadece yetkili kullanıcılar)
        /// </summary>
        [HttpPost("user/{userId}/statistics")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<SalesStatisticsDto>> GetUserStatistics(Guid userId, [FromBody] SalesStatisticsRequestDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // Kullanıcının bu bilgiyi görme yetkisi var mı kontrol et
                var accessibleUsers = await _salesStatisticsService.GetManagedUsersAsync(currentUserId);
                var canAccess = userId == currentUserId ||
                               accessibleUsers.Any(u => u.Id == userId) ||
                               User.IsInRole("Admin");

                if (!canAccess)
                {
                    return Forbid("Bu kullanıcının istatistiklerini görme yetkiniz yok");
                }

                var statistics = await _salesStatisticsService.GetUserSalesStatisticsAsync(userId, request);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Takım satış istatistiklerini getirir (Manager ve Admin için)
        /// </summary>
        [HttpPost("team-statistics")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<TeamSalesStatisticsDto>> GetTeamStatistics([FromBody] SalesStatisticsRequestDto request)
        {
            try
            {
                var managerId = GetCurrentUserId();
                var statistics = await _salesStatisticsService.GetTeamSalesStatisticsAsync(managerId, request);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Dashboard için genel istatistikleri getirir
        /// </summary>
        [HttpPost("dashboard")]
        public async Task<ActionResult<DashboardStatisticsDto>> GetDashboardStatistics([FromBody] SalesStatisticsRequestDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var statistics = await _salesStatisticsService.GetDashboardStatisticsAsync(currentUserId, request);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Kullanıcının erişim yetkisi olan standları getirir
        /// </summary>
        [HttpGet("accessible-stands")]
        public async Task<ActionResult> GetAccessibleStands()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var stands = await _salesStatisticsService.GetAccessibleStandsAsync(currentUserId);
                return Ok(stands);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
    }
}
/// Kullanıcının kendi satış istatistiklerini getirir
