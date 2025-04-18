using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Domain.Entities;
using FuarYonetimSistemi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
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
    }

}
