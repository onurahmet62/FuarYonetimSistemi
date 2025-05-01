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
                .Where(p => !p.IsDeleted)
                .Include(p => p.Stand)
                .ToListAsync();
        }

        public async Task<Payment> GetByIdAsync(Guid id)
        {
            return await _context.Payments
                .Include(p => p.Stand)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<Payment> AddAsync(PaymentCreateDto dto)
        {
            var stand = await _context.Stands
                .FirstOrDefaultAsync(s => s.Id == dto.StandId && !s.IsDeleted);

            if (stand == null)
                throw new Exception("Stand bulunamadı.");

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                PaymentDate = dto.PaymentDate,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                PaymentDescription = dto.PaymentDescription,
                ReceivedBy = dto.ReceivedBy,
                StandId = dto.StandId,
                Stand = stand,
                IsDeleted = false
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> UpdateAsync(Guid id, Payment updatedPayment)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null || payment.IsDeleted) return null;

            payment.PaymentDate = updatedPayment.PaymentDate;
            payment.Amount = updatedPayment.Amount;
            payment.PaymentMethod = updatedPayment.PaymentMethod;
            payment.PaymentDescription = updatedPayment.PaymentDescription;
            payment.ReceivedBy = updatedPayment.ReceivedBy;

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
    }
}
