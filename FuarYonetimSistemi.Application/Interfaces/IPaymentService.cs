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
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<Payment?> GetByIdAsync(Guid id);
        Task<Payment> AddAsync(PaymentCreateDto dto);
        Task<Payment?> UpdateAsync(Guid id, Payment updatedPayment);
        Task<bool> DeleteAsync(Guid id);

        Task<IEnumerable<Payment>> GetFilteredAsync(PaymentFilterDto filterDto);

        Task<PaymentWithStandAndFairDto?> GetWithStandAndFairAsync(Guid id);

    }
}
