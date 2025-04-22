using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Domain.Entities;
using FuarYonetimSistemi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Services
{
    // ParticipantService.cs
    public class ParticipantService : IParticipantService
    {
        private readonly AppDbContext _context;

        public ParticipantService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ParticipantDto> CreateAsync(CreateParticipantDto dto)
        {
            var participant = new Participant
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                CreateDate = DateTime.UtcNow,
                AuthFullName = dto.AuthFullName,
                IsDeleted = false
            };

            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();

            return new ParticipantDto
            {
                Id = participant.Id,
                FullName = participant.FullName,
                Email = participant.Email,
                Phone = participant.Phone,
                CreateDate = participant.CreateDate,
                AuthFullName = participant.AuthFullName
            };
        }

        public async Task<ParticipantDto> UpdateAsync(Guid id, UpdateParticipantDto dto)
        {
            var participant = await _context.Participants.FindAsync(id);
            if (participant == null || participant.IsDeleted)
                return null;

            participant.FullName = dto.FullName;
            participant.Email = dto.Email;
            participant.Phone = dto.Phone;
            participant.AuthFullName = dto.AuthFullName;
            participant.CreateDate = DateTime.UtcNow;


            await _context.SaveChangesAsync();

            return new ParticipantDto
            {
                Id = participant.Id,
                FullName = participant.FullName,
                Email = participant.Email,
                Phone = participant.Phone,  
                CreateDate = DateTime.UtcNow,
                AuthFullName = participant.AuthFullName
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var participant = await _context.Participants.FindAsync(id);
            if (participant == null || participant.IsDeleted)
                return false;

            participant.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ParticipantDto> GetByIdAsync(Guid id)
        {
            var participant = await _context.Participants
                .Where(p => p.Id == id && !p.IsDeleted)
                .FirstOrDefaultAsync();

            if (participant == null)
                return null;

            return new ParticipantDto
            {
                Id = participant.Id,
                FullName = participant.FullName,
                Email = participant.Email,
                Phone = participant.Phone, 
                CreateDate = DateTime.UtcNow,
                AuthFullName = participant.AuthFullName

            };
        }

        public async Task<IEnumerable<ParticipantDto>> GetAllAsync()
        {
            return await _context.Participants
                .Where(p => !p.IsDeleted)
                .Select(p => new ParticipantDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    Email = p.Email,
                    Phone = p.Phone
                })
                .ToListAsync();
        }

        public async Task<PagedResult<ParticipantDto>> FilterPagedAsync(ParticipantFilterDto filter)
        {
            var query = _context.Participants
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.FullName))
                query = query.Where(p => p.FullName.Contains(filter.FullName));

            if (!string.IsNullOrWhiteSpace(filter.Email))
                query = query.Where(p => p.Email.Contains(filter.Email));

            if (!string.IsNullOrWhiteSpace(filter.Phone))
                query = query.Where(p => p.Phone.Contains(filter.Phone));

            if (!string.IsNullOrWhiteSpace(filter.AuthFullName))
                query = query.Where(p => p.AuthFullName.Contains(filter.AuthFullName));

            if (filter.CreateDate.HasValue)
                query = query.Where(p => p.CreateDate.Date == filter.CreateDate.Value.Date);

            // Dinamik sıralama
            query = filter.SortBy?.ToLower() switch
            {
                "fullname" => filter.IsDescending ? query.OrderByDescending(p => p.FullName) : query.OrderBy(p => p.FullName),
                "email" => filter.IsDescending ? query.OrderByDescending(p => p.Email) : query.OrderBy(p => p.Email),
                "phone" => filter.IsDescending ? query.OrderByDescending(p => p.Phone) : query.OrderBy(p => p.Phone),
                "authfullname" => filter.IsDescending ? query.OrderByDescending(p => p.AuthFullName) : query.OrderBy(p => p.AuthFullName),
                "createdate" => filter.IsDescending ? query.OrderByDescending(p => p.CreateDate) : query.OrderBy(p => p.CreateDate),
                _ => filter.IsDescending ? query.OrderByDescending(p => p.CreateDate) : query.OrderBy(p => p.CreateDate)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(p => new ParticipantDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    Email = p.Email,
                    Phone = p.Phone,
                    CreateDate = p.CreateDate,
                    AuthFullName = p.AuthFullName
                })
                .ToListAsync();

            return new PagedResult<ParticipantDto>
            {
                Items = items,
                TotalCount = totalCount
            };
        }



    }


}
