using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<Payment?> GetByIdAsync(Guid id);
        Task<Payment> AddAsync(PaymentCreateDto dto);
        Task<Payment?> UpdateAsync(Guid id, PaymentUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<PagedResult<Payment>> GetFilteredAsync(PaymentFilterDto filterDto);
        Task<PaymentWithStandAndFairDto?> GetWithStandAndFairAsync(Guid id);
    }
}
