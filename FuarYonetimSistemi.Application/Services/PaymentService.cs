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
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> AddAsync(Payment payment)
        {
            // Sadece StandId'yi kullanıyoruz, stand'ı doğrulamak için
            var stand = await _context.Stands.FindAsync(payment.StandId);
            if (stand == null)
            {
                throw new Exception("Stand bulunamadı.");
            }

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }



        public async Task<bool> DeleteAsync(Guid id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return false;  // Ödeme bulunamadı
            }

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Payment>> FilterAsync(string searchTerm)
        {
            return await _context.Payments
                .Include(p => p.Stand)
              
                .Where(p => p.PaymentDescription.Contains(searchTerm) || p.PaymentMethod.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Stand)  // Stand bilgisi
                  // Katılımcı bilgisi
                .ToListAsync();
        }

        public async Task<Payment> GetByIdAsync(Guid id)
        {
            return await _context.Payments
                .Include(p => p.Stand)
               
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Payment>> GetByParticipantIdAsync(Guid participantId)
        {
            return await _context.Payments
                .Include(p => p.Stand)
              
                
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByStandIdAsync(Guid standId)
        {
            return await _context.Payments
                .Include(p => p.Stand)
               
                .Where(p => p.StandId == standId)
                .ToListAsync();
        }

        public async Task<Payment> UpdateAsync(Guid id, Payment updatedPayment)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return null;  // Ödeme bulunamadı
            }

            // Verileri güncelle
            payment.PaymentDate = updatedPayment.PaymentDate;
            payment.Amount = updatedPayment.Amount;
            payment.PaymentMethod = updatedPayment.PaymentMethod;
            payment.PaymentDescription = updatedPayment.PaymentDescription;
            payment.ReceivedBy = updatedPayment.ReceivedBy;

            // Değişiklikleri kaydet
            await _context.SaveChangesAsync();
            return payment;
        }
    }
}
