using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<Payment>> GetAllAsync();  // Tüm ödemeleri getir
        Task<Payment> GetByIdAsync(Guid id);  // Id'ye göre ödeme getir
        Task<IEnumerable<Payment>> GetByStandIdAsync(Guid standId);  // Stand'a ait ödemeleri getir
        Task<IEnumerable<Payment>> GetByParticipantIdAsync(Guid participantId);  // Katılımcıya ait ödemeleri getir
        Task<Payment> AddAsync(Payment payment);  // Yeni bir ödeme ekle
        Task<Payment> UpdateAsync(Guid id, Payment updatedPayment);  // Ödeme güncelle
        Task<bool> DeleteAsync(Guid id);  // Ödeme sil
        Task<IEnumerable<Payment>> FilterAsync(string searchTerm);  // Filtreleme işlemi (Ödeme açıklaması veya ödeme metodu)
    }
}
