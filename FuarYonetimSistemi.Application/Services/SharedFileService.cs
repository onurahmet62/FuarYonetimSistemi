using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Domain.Entities;
using FuarYonetimSistemi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Services
{
    public class SharedFileService : ISharedFileService
    {
        private readonly AppDbContext _context;

        public SharedFileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SharedFile>> GetAllFilesAsync()
        {
            return await _context.SharedFiles
                .Where(f => !f.IsDeleted)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<SharedFile> GetFileByIdAsync(Guid id)
        {
            return await _context.SharedFiles
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);
        }

        public async Task<SharedFile> CreateFileAsync(SharedFileCreateDto dto)
        {
            var file = new SharedFile
            {
                Id = Guid.NewGuid(),
                FileName = dto.FileName,
                FilePath = dto.FilePath,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.SharedFiles.Add(file);
            await _context.SaveChangesAsync();
            return file;
        }

        public async Task<bool> UpdateFileAsync(Guid id, SharedFileCreateDto dto)
        {
            var file = await _context.SharedFiles.FindAsync(id);
            if (file == null || file.IsDeleted) return false;

            file.FileName = dto.FileName;
            file.FilePath = dto.FilePath;
            file.UpdatedAt = DateTime.UtcNow;

            _context.SharedFiles.Update(file);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteFileAsync(Guid id)
        {
            var file = await _context.SharedFiles.FindAsync(id);
            if (file == null || file.IsDeleted) return false;

            file.IsDeleted = true;
            file.UpdatedAt = DateTime.UtcNow;

            _context.SharedFiles.Update(file);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
