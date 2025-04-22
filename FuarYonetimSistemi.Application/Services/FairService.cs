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

                    CategoryName = f.Category != null ? f.Category.Name : null
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
                

                CategoryName = fair.Category != null ? fair.Category.Name : null
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

                CategoryId = fairDto.CategoryId ?? Guid.Empty,
                IsDeleted = false
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

                CategoryName = (await _context.Categories.FindAsync(fair.CategoryId))?.Name
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

            if (!string.IsNullOrEmpty(filterDto.Location))
                query = query.Where(f => f.Location.Contains(filterDto.Location));

            if (filterDto.CategoryId.HasValue)
                query = query.Where(f => f.CategoryId == filterDto.CategoryId);

            if (filterDto.Year.HasValue)
                query = query.Where(f => f.Year == filterDto.Year);

            if (filterDto.StartDateFrom.HasValue)
                query = query.Where(f => f.StartDate >= filterDto.StartDateFrom.Value);

            if (filterDto.StartDateTo.HasValue)
                query = query.Where(f => f.StartDate <= filterDto.StartDateTo.Value);

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
                    CategoryName = f.Category.Name
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
    }

}
