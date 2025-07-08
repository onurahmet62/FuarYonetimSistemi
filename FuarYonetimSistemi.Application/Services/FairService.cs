using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Domain.Entities;
using FuarYonetimSistemi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FuarYonetimSistemi.Application.Services
{
    public class FairService : IFairService
    {
        private readonly AppDbContext _context;

        public FairService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FairDto>> GetAllFairsAsync()
        {
            return await _context.Fairs
                .Where(f => !f.IsDeleted)
                .Include(f => f.Category)
                .Select(f => new FairDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Location = f.Location,
                    Year = f.Year,
                    StartDate = f.StartDate,
                    EndDate = f.EndDate,
                    Organizer = f.Organizer,
                    CategoryName = f.Category != null ? f.Category.Name : null,

                    FairType = f.FairType,
                    Website = f.Website,
                    Email = f.Email,
                    TotalParticipantCount = f.TotalParticipantCount,
                    ForeignParticipantCount = f.ForeignParticipantCount,
                    TotalVisitorCount = f.TotalVisitorCount,
                    ForeignVisitorCount = f.ForeignVisitorCount,
                    TotalStandArea = f.TotalStandArea,
                    ParticipatingCountries = f.ParticipatingCountries,
                    Budget = f.Budget,

                    RevenueTarget = f.RevenueTarget,
                    ExpenseTarget = f.ExpenseTarget,
                    NetProfitTarget = f.NetProfitTarget,
                    ActualRevenue = f.ActualRevenue,
                    ActualExpense = f.ActualExpense,
                    ActualNetProfit = f.ActualNetProfit
                })
                .ToListAsync();
        }


        public async Task<FairDto> GetFairByIdAsync(Guid id)
        {
            var fair = await _context.Fairs
                .Include(f => f.Category)
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);

            if (fair == null)
                return null;

            return new FairDto
            {
                Id = fair.Id,
                Name = fair.Name,
                Location = fair.Location,
                Year = fair.Year,
                StartDate = fair.StartDate,
                EndDate = fair.EndDate,
                Organizer = fair.Organizer,
                CategoryName = fair.Category != null ? fair.Category.Name : null,

                FairType = fair.FairType,
                Website = fair.Website,
                Email = fair.Email,
                TotalParticipantCount = fair.TotalParticipantCount,
                ForeignParticipantCount = fair.ForeignParticipantCount,
                TotalVisitorCount = fair.TotalVisitorCount,
                ForeignVisitorCount = fair.ForeignVisitorCount,
                TotalStandArea = fair.TotalStandArea,
                ParticipatingCountries = fair.ParticipatingCountries,
                Budget = fair.Budget,

                RevenueTarget = fair.RevenueTarget,
                ExpenseTarget = fair.ExpenseTarget,
                NetProfitTarget = fair.NetProfitTarget,
                ActualRevenue = fair.ActualRevenue,
                ActualExpense = fair.ActualExpense,
                ActualNetProfit = fair.ActualNetProfit
            };
        }

        public async Task<FairDto> CreateFairAsync(FairCreateDto fairDto)
        {
            var fair = new Fair
            {
                Id = Guid.NewGuid(),
                Name = fairDto.Name,
                Location = fairDto.Location,
                Year = fairDto.Year,
                StartDate = fairDto.StartDate,
                EndDate = fairDto.EndDate,
                Organizer = fairDto.Organizer,
                CategoryId = fairDto.CategoryId,

                FairType = fairDto.FairType,
                Website = fairDto.Website,
                Email = fairDto.Email,
                TotalParticipantCount = fairDto.TotalParticipantCount,
                ForeignParticipantCount = fairDto.ForeignParticipantCount,
                TotalVisitorCount = fairDto.TotalVisitorCount,
                ForeignVisitorCount = fairDto.ForeignVisitorCount,
                TotalStandArea = fairDto.TotalStandArea,
                ParticipatingCountries = fairDto.ParticipatingCountries,
                Budget = fairDto.Budget,

                RevenueTarget = fairDto.RevenueTarget,
                ExpenseTarget = fairDto.ExpenseTarget,
                NetProfitTarget = fairDto.NetProfitTarget,

                ActualRevenue = fairDto.ActualRevenue, // Dışarıdan set ediliyorsa, yoksa 0 olabilir
                ActualExpense = fairDto.ActualExpense,
                ActualNetProfit = fairDto.ActualNetProfit,

                IsDeleted = false,
            };

            _context.Fairs.Add(fair);
            await _context.SaveChangesAsync();

            return new FairDto
            {
                Id = fair.Id,
                Name = fair.Name,
                Location = fair.Location,
                Year = fair.Year,
                StartDate = fair.StartDate,
                EndDate = fair.EndDate,
                Organizer = fair.Organizer,
                CategoryName = (await _context.Categories.FindAsync(fair.CategoryId))?.Name,

                FairType = fair.FairType,
                Website = fair.Website,
                Email = fair.Email,
                TotalParticipantCount = fair.TotalParticipantCount,
                ForeignParticipantCount = fair.ForeignParticipantCount,
                TotalVisitorCount = fair.TotalVisitorCount,
                ForeignVisitorCount = fair.ForeignVisitorCount,
                TotalStandArea = fair.TotalStandArea,
                ParticipatingCountries = fair.ParticipatingCountries,
                Budget = fair.Budget,

                RevenueTarget = fair.RevenueTarget,
                ExpenseTarget = fair.ExpenseTarget,
                NetProfitTarget = fair.NetProfitTarget,

                ActualRevenue = fair.ActualRevenue,
                ActualExpense = fair.ActualExpense,
                ActualNetProfit = fair.ActualNetProfit,
            };
        }


        public async Task<bool> DeleteFairAsync(Guid id)
        {
            var fair = await _context.Fairs.FindAsync(id);
            if (fair == null || fair.IsDeleted)
                return false;

            fair.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(IEnumerable<FairDto> fairs, int totalCount)> GetFilteredFairsAsync(FairFilterDto filterDto)
        {
            var query = _context.Fairs
                .Include(f => f.Category)
                .Where(f => !f.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filterDto.Name))
                query = query.Where(f => f.Name.Contains(filterDto.Name));

            if (!string.IsNullOrEmpty(filterDto.Organizer))
                query = query.Where(f => f.Organizer.Contains(filterDto.Organizer));

            if (!string.IsNullOrEmpty(filterDto.Location))
                query = query.Where(f => f.Location.Contains(filterDto.Location));

            if (filterDto.Year.HasValue)
                query = query.Where(f => f.Year == filterDto.Year);

            if (!string.IsNullOrEmpty(filterDto.FairType))
                query = query.Where(f => f.FairType == filterDto.FairType);

            if (filterDto.CategoryId.HasValue)
                query = query.Where(f => f.CategoryId == filterDto.CategoryId);

            // Sıralama
            if (!string.IsNullOrWhiteSpace(filterDto.SortBy))
            {
                switch (filterDto.SortBy.ToLower())
                {
                    case "name":
                        query = filterDto.SortDescending ? query.OrderByDescending(f => f.Name) : query.OrderBy(f => f.Name);
                        break;
                    case "year":
                        query = filterDto.SortDescending ? query.OrderByDescending(f => f.Year) : query.OrderBy(f => f.Year);
                        break;
                    case "location":
                        query = filterDto.SortDescending ? query.OrderByDescending(f => f.Location) : query.OrderBy(f => f.Location);
                        break;
                    case "organizer":
                        query = filterDto.SortDescending ? query.OrderByDescending(f => f.Organizer) : query.OrderBy(f => f.Organizer);
                        break;
                    default:
                        query = query.OrderBy(f => f.Name);
                        break;
                }
            }

            var totalCount = await query.CountAsync();

            var fairs = await query
                .Skip((filterDto.PageNumber - 1) * filterDto.PageSize)
                .Take(filterDto.PageSize)
                .Select(f => new FairDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Location = f.Location,
                    Year = f.Year,
                    StartDate = f.StartDate,
                    EndDate = f.EndDate,
                    Organizer = f.Organizer,
                    CategoryName = f.Category != null ? f.Category.Name : null,

                    FairType = f.FairType,
                    Website = f.Website,
                    Email = f.Email,
                    TotalParticipantCount = f.TotalParticipantCount,
                    ForeignParticipantCount = f.ForeignParticipantCount,
                    TotalVisitorCount = f.TotalVisitorCount,
                    ForeignVisitorCount = f.ForeignVisitorCount,
                    TotalStandArea = f.TotalStandArea,
                    ParticipatingCountries = f.ParticipatingCountries,
                    Budget = f.Budget,

                    RevenueTarget = f.RevenueTarget,
                    ExpenseTarget = f.ExpenseTarget,
                    NetProfitTarget = f.NetProfitTarget,
                    ActualRevenue = f.ActualRevenue,
                    ActualExpense = f.ActualExpense,
                    ActualNetProfit = f.ActualNetProfit
                })
                .ToListAsync();

            return (fairs, totalCount);
        }


        public async Task<byte[]> ExportFairsToExcelAsync(FairFilterDto filterDto)
        {
            var (fairs, _) = await GetFilteredFairsAsync(filterDto);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Fuarlar");

            sheet.Cells[1, 1].Value = "Adı";
            sheet.Cells[1, 2].Value = "Lokasyon";
            sheet.Cells[1, 3].Value = "Yıl";
            sheet.Cells[1, 4].Value = "Başlangıç";
            sheet.Cells[1, 5].Value = "Bitiş";
            sheet.Cells[1, 6].Value = "Kategori";

            int row = 2;
            foreach (var fair in fairs)
            {
                sheet.Cells[row, 1].Value = fair.Name;
                sheet.Cells[row, 2].Value = fair.Location;
                sheet.Cells[row, 3].Value = fair.Year;
                sheet.Cells[row, 4].Value = fair.StartDate.ToShortDateString();
                sheet.Cells[row, 5].Value = fair.EndDate.ToShortDateString();
                sheet.Cells[row, 6].Value = fair.CategoryName;
                row++;
            }

            return package.GetAsByteArray();
        }

        public async Task<IEnumerable<FairParticipationDto>> GetFairsByParticipantIdAsync(Guid participantId)
        {
            return await _context.Stands
                .Include(s => s.Fair).ThenInclude(f => f.Category)
                .Where(s => s.ParticipantId == participantId && !s.Fair.IsDeleted)
                .Select(s => new FairParticipationDto
                {
                    FairId = s.Fair.Id,
                    FairName = s.Fair.Name,
                    Location = s.Fair.Location,
                    Year = (int)s.Fair.Year,
                    StartDate = s.Fair.StartDate,
                    EndDate = s.Fair.EndDate,
                    Organizer = s.Fair.Organizer,
                    CategoryName = s.Fair.Category != null ? s.Fair.Category.Name : null
                })
                .Distinct()
                .ToListAsync();
        }

        public async Task<FairDto?> UpdateFairAsync(FairUpdateDto fairDto)
        {
            var fair = await _context.Fairs.FirstOrDefaultAsync(f => f.Id == fairDto.Id && !f.IsDeleted);
            if (fair == null) return null;

            // Yeni kategori adı verildiyse işle
            if (!string.IsNullOrWhiteSpace(fairDto.NewCategoryName))
            {
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name == fairDto.NewCategoryName);

                if (existingCategory != null)
                    fairDto.CategoryId = existingCategory.Id;
                else
                {
                    var newCategory = new Category { Id = Guid.NewGuid(), Name = fairDto.NewCategoryName };
                    _context.Categories.Add(newCategory);
                    await _context.SaveChangesAsync(); // kategori id’ye sahip olmak için
                    fairDto.CategoryId = newCategory.Id;
                }
            }

            // Alanları güncelle
            fair.Name = fairDto.Name;
            fair.Location = fairDto.Location;
            fair.Organizer = fairDto.Organizer;
            fair.Year = fairDto.Year;
            fair.StartDate = fairDto.StartDate;
            fair.EndDate = fairDto.EndDate;
            fair.CategoryId = fairDto.CategoryId;

            fair.FairType = fairDto.FairType;
            fair.Website = fairDto.Website;
            fair.Email = fairDto.Email;
            fair.TotalParticipantCount = fairDto.TotalParticipantCount;
            fair.ForeignParticipantCount = fairDto.ForeignParticipantCount;
            fair.TotalVisitorCount = fairDto.TotalVisitorCount;
            fair.ForeignVisitorCount = fairDto.ForeignVisitorCount;
            fair.TotalStandArea = fairDto.TotalStandArea;
            fair.ParticipatingCountries = fairDto.ParticipatingCountries;
            fair.Budget = fairDto.Budget;
            fair.RevenueTarget = fairDto.RevenueTarget;
            fair.ExpenseTarget = fairDto.ExpenseTarget;
            fair.NetProfitTarget = fairDto.NetProfitTarget;
            fair.ActualRevenue = fairDto.ActualRevenue;
            fair.ActualExpense = fairDto.ActualExpense;
            fair.ActualNetProfit = fairDto.ActualNetProfit;

            await _context.SaveChangesAsync();

            return await GetFairByIdAsync(fair.Id); // tekrar DTO'ya maplenmiş hali
        }


    }

}
