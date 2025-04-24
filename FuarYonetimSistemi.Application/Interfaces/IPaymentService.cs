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
        Task<List<Payment>> GetAllAsync();
        Task<Payment> GetByIdAsync(Guid id);
        Task<List<Payment>> GetByParticipantIdAsync(Guid participantId);
        Task<List<Payment>> GetByStandIdAsync(Guid standId);
        Task<Payment> AddAsync(Payment payment);
        Task DeleteAsync(Guid id);
    }
}
