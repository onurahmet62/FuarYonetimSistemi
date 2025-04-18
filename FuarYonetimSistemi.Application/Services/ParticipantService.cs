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
                IsDeleted = false
            };

            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();

            return new ParticipantDto
            {
                Id = participant.Id,
                FullName = participant.FullName,
                Email = participant.Email,
                Phone = participant.Phone
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

            await _context.SaveChangesAsync();

            return new ParticipantDto
            {
                Id = participant.Id,
                FullName = participant.FullName,
                Email = participant.Email,
                Phone = participant.Phone
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
                Phone = participant.Phone
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
    }


}
