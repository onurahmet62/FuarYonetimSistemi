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

        public async Task<List<Payment>> GetAllAsync() => await _context.Payments.ToListAsync();

        public async Task<Payment> GetByIdAsync(Guid id) => await _context.Payments.FindAsync(id);

        public async Task<List<Payment>> GetByParticipantIdAsync(Guid participantId)
        {
            return await _context.Payments.Where(p => p.Id == participantId).ToListAsync();
        }

        public async Task<List<Payment>> GetByStandIdAsync(Guid standId)
        {
            return await _context.Payments.Where(p => p.StandId == standId).ToListAsync();
        }

        public async Task<Payment> AddAsync(Payment payment)
        {
            var stand = await _context.Stands.FindAsync(payment.StandId);
            if (stand == null) throw new Exception("Stand bulunamadı.");

            stand.AmountPaid += payment.Amount;
            stand.AmountRemaining = stand.Price - stand.AmountPaid;

            if (stand.AmountPaid == 0)
                stand.PaymentStatus = "Hiç Ödenmedi";
            else if (stand.AmountPaid < stand.Price)
                stand.PaymentStatus = "Kısmi Ödendi";
            else
                stand.PaymentStatus = "Tamamen Ödendi";

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task DeleteAsync(Guid id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) throw new Exception("Ödeme bulunamadı.");

            var stand = await _context.Stands.FindAsync(payment.StandId);
            if (stand != null)
            {
                stand.AmountPaid -= payment.Amount;
                stand.AmountRemaining = stand.Price - stand.AmountPaid;
                stand.PaymentStatus = stand.AmountPaid == 0 ? "Hiç Ödenmedi" : (stand.AmountPaid < stand.Price ? "Kısmi Ödendi" : "Tamamen Ödendi");
            }

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
        }
    }
}
