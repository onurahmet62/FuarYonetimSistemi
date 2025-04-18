using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Domain.Entities;
using FuarYonetimSistemi.Domain.Enums;  // PaymentStatus enum kullanımı için gerekli
using FuarYonetimSistemi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Services
{
    public class StandService : IStandService
    {
        private readonly AppDbContext _context;

        public StandService(AppDbContext context)
        {
            _context = context;
        }

        // Stand oluşturma
        public async Task<StandDto> CreateStandAsync(StandCreateDto standCreateDto)
        {
            var stand = new Stand
            {
                Id = Guid.NewGuid(),
                Name = standCreateDto.Name,
                Price = standCreateDto.Price,
                Area = standCreateDto.Area, // decimal olarak kaydedilecek
                FairId = standCreateDto.FairId,
                ParticipantId = standCreateDto.ParticipantId,
                Description = standCreateDto.Description,
                PaymentStatus = PaymentStatus.PaymentNotReceived.ToString(),  // Enum'daki mevcut değeri kullanın
            };

            await _context.Stands.AddAsync(stand);
            await _context.SaveChangesAsync();

            return new StandDto
            {
                Id = stand.Id,
                Name = stand.Name,
                Price = stand.Price,  // decimal'den double'a dönüşüm
                Area = stand.Area,   // decimal'den double'a dönüşüm
                Description = stand.Description,
                ParticipantFullName = stand.Participant?.FullName,  // Optional olarak erişim
                FairName = stand.Fair?.Name,  // Optional olarak erişim
                PaymentStatus = stand.PaymentStatus.ToString(),  // Enum'dan string'e dönüşüm
                AmountPaid = stand.AmountPaid,  // decimal'den double'a dönüşüm
                AmountRemaining = stand.AmountRemaining,  // decimal'den double'a dönüşüm
                DueDate = stand.DueDate ?? DateTime.MinValue  // Nullable DateTime kontrolü
            };
        }


        // Stand güncelleme
        public async Task<StandDto> UpdateStandAsync(StandUpdateDto standUpdateDto)
        {
            var stand = await _context.Stands.FindAsync(standUpdateDto.Id);
            if (stand == null)
                throw new Exception("Stand bulunamadı.");

            stand.Name = standUpdateDto.Name;
            stand.Price = standUpdateDto.Price;
            stand.Description = standUpdateDto.Description;

            _context.Stands.Update(stand);
            await _context.SaveChangesAsync();

            return new StandDto
            {
                Id = stand.Id,
                Name = stand.Name,
                Price = stand.Price,
                Description = stand.Description,
                ParticipantFullName = stand.Participant?.FullName,
                FairName = stand.Fair?.Name,
                PaymentStatus = stand.PaymentStatus.ToString(),
                AmountPaid = stand.AmountPaid,
                AmountRemaining = stand.AmountRemaining,
                DueDate = stand.DueDate ?? DateTime.MinValue
            };
        }

        // Stand silme
        public async Task<bool> DeleteStandAsync(Guid standId)
        {
            var stand = await _context.Stands.FindAsync(standId);
            if (stand == null)
                return false;

            _context.Stands.Remove(stand);
            await _context.SaveChangesAsync();
            return true;
        }

        // Bir fuara ait standları listeleme
        public async Task<List<StandDto>> GetStandsByFairAsync(Guid fairId)
        {
            var stands = await _context.Stands
                .Where(s => s.FairId == fairId)
                .Include(s => s.Participant)  // İlişkili veriyi dahil et
                .Include(s => s.Fair)  // İlişkili veriyi dahil et
                .ToListAsync();

            return stands.Select(stand => new StandDto
            {
                Id = stand.Id,
                Name = stand.Name,
                Price = stand.Price,
                Area = stand.Area,
                Description = stand.Description,
                ParticipantFullName = stand.Participant?.FullName,
                FairName = stand.Fair?.Name,
                PaymentStatus = stand.PaymentStatus.ToString(),
                AmountPaid = stand.AmountPaid,
                AmountRemaining = stand.AmountRemaining,
                DueDate = stand.DueDate ?? DateTime.MinValue
            }).ToList();
        }

        // Bir katılımcıya ait standları listeleme
        public async Task<List<StandDto>> GetStandsByParticipantAsync(Guid participantId)
        {
            var stands = await _context.Stands
                .Where(s => s.ParticipantId == participantId)
                .Include(s => s.Participant)
                .Include(s => s.Fair)
                .ToListAsync();

            return stands.Select(stand => new StandDto
            {
                Id = stand.Id,
                Name = stand.Name,
                Price = stand.Price,
                Area = stand.Area,
                Description = stand.Description,
                ParticipantFullName = stand.Participant?.FullName,
                FairName = stand.Fair?.Name,
                PaymentStatus = stand.PaymentStatus.ToString(),
                AmountPaid = stand.AmountPaid,
                AmountRemaining = stand.AmountRemaining,
                DueDate = stand.DueDate ?? DateTime.MinValue
            }).ToList();
        }

        // Stand ödeme durumu güncelleme
        public async Task<StandDto> UpdatePaymentStatusAsync(StandPaymentStatusDto paymentStatusDto)
        {
            var stand = await _context.Stands.FindAsync(paymentStatusDto.StandId);
            if (stand == null)
                throw new Exception("Stand bulunamadı.");

            stand.PaymentStatus = paymentStatusDto.PaymentStatus;
            stand.AmountPaid = paymentStatusDto.AmountPaid;
            stand.AmountRemaining = paymentStatusDto.AmountRemaining;
            stand.DueDate = paymentStatusDto.DueDate;

            _context.Stands.Update(stand);
            await _context.SaveChangesAsync();

            return new StandDto
            {
                Id = stand.Id,
                Name = stand.Name,
                Price = stand.Price,
                Area = stand.Area,
                Description = stand.Description,
                ParticipantFullName = stand.Participant?.FullName,
                FairName = stand.Fair?.Name,
                PaymentStatus = stand.PaymentStatus.ToString(),
                AmountPaid = stand.AmountPaid,
                AmountRemaining = stand.AmountRemaining,
                DueDate = stand.DueDate ?? DateTime.MinValue
            };
        }

        // Stand detaylarını alma
        public async Task<StandDto> GetStandByIdAsync(Guid standId)
        {
            var stand = await _context.Stands
                .Include(s => s.Participant)  // İlişkili veriyi dahil et
                .Include(s => s.Fair)  // İlişkili veriyi dahil et
                .FirstOrDefaultAsync(s => s.Id == standId);

            if (stand == null)
                throw new Exception("Stand bulunamadı.");

            return new StandDto
            {
                Id = stand.Id,
                Name = stand.Name,
                Price = stand.Price,
                Area = stand.Area,
                Description = stand.Description,
                ParticipantFullName = stand.Participant?.FullName,
                FairName = stand.Fair?.Name,
                PaymentStatus = stand.PaymentStatus.ToString(),
                AmountPaid =stand.AmountPaid,
                AmountRemaining = stand.AmountRemaining,
                DueDate = stand.DueDate ?? DateTime.MinValue
            };
        }
    }
}
