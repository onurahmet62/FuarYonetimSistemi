using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Domain.Entities;
using FuarYonetimSistemi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Stand)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<Payment?> GetByIdAsync(Guid id)
        {
            return await _context.Payments
                .Include(p => p.Stand)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<Payment> AddAsync(PaymentCreateDto dto)
        {
            var stand = await _context.Stands.FirstOrDefaultAsync(s => s.Id == dto.StandId && !s.IsDeleted);
            if (stand == null) throw new Exception("Stand bulunamadı.");

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                PaymentDate = dto.PaymentDate,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                PaymentDescription = dto.PaymentDescription,
                ReceivedBy = dto.ReceivedBy,
                StandId = dto.StandId,
                IsDeleted = false
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> UpdateAsync(Guid id, PaymentUpdateDto dto)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null || payment.IsDeleted) return null;

            payment.PaymentDate = dto.PaymentDate;
            payment.Amount = dto.Amount;
            payment.PaymentMethod = dto.PaymentMethod;
            payment.PaymentDescription = dto.PaymentDescription;
            payment.ReceivedBy = dto.ReceivedBy;

            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null || payment.IsDeleted) return false;

            payment.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<Payment>> GetFilteredAsync(PaymentFilterDto filter)
        {
            var query = _context.Payments
                .Include(p => p.Stand)
                    .ThenInclude(s => s.Participant)
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            if (filter.StartDate.HasValue)
                query = query.Where(p => p.PaymentDate >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(p => p.PaymentDate <= filter.EndDate.Value);

            if (!string.IsNullOrWhiteSpace(filter.PaymentMethod))
                query = query.Where(p => p.PaymentMethod == filter.PaymentMethod);

            if (!string.IsNullOrWhiteSpace(filter.ReceivedBy))
                query = query.Where(p => p.ReceivedBy.Contains(filter.ReceivedBy));

            if (filter.ParticipantId.HasValue)
                query = query.Where(p => p.Stand.ParticipantId == filter.ParticipantId.Value);

            var totalCount = await query.CountAsync();

            query = filter.SortBy switch
            {
                "Amount" => filter.SortDescending ? query.OrderByDescending(p => p.Amount) : query.OrderBy(p => p.Amount),
                "ReceivedBy" => filter.SortDescending ? query.OrderByDescending(p => p.ReceivedBy) : query.OrderBy(p => p.ReceivedBy),
                _ => filter.SortDescending ? query.OrderByDescending(p => p.PaymentDate) : query.OrderBy(p => p.PaymentDate),
            };

            query = query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize);

            return new PagedResult<Payment>
            {
                Items = await query.ToListAsync(),
                TotalCount = totalCount
            };
        }

        public async Task<PaymentWithStandAndFairDto?> GetWithStandAndFairAsync(Guid id)
        {
            var payment = await _context.Payments
                .Include(p => p.Stand)
                    .ThenInclude(s => s.Fair)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (payment == null) return null;

            return new PaymentWithStandAndFairDto
            {
                Id = payment.Id,
                PaymentDate = payment.PaymentDate,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                PaymentDescription = payment.PaymentDescription,
                StandId = payment.StandId,
                IsDeleted = payment.IsDeleted,
                ReceivedBy = payment.ReceivedBy,
                Stand = new StandDto
                {
                    Id = payment.Stand.Id,
                    Name = payment.Stand.Name,
                    ContractArea = (double)payment.Stand.ContractArea,
                    UnitPrice = (decimal)payment.Stand.UnitPrice
                },
                Fair = new FairDto
                {
                    Id = payment.Stand.Fair.Id,
                    Name = payment.Stand.Fair.Name,
                    StartDate = payment.Stand.Fair.StartDate,
                    EndDate = payment.Stand.Fair.EndDate
                }
            };
        }
    }
}
