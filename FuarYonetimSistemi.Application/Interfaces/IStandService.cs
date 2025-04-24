using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Interfaces
{
    public interface IStandService
    {
        // Stand oluşturma
        Task<StandDto> CreateStandAsync(StandCreateDto standCreateDto);

        // Stand bilgilerini güncelleme
        Task<StandDto> UpdateStandAsync(StandUpdateDto standUpdateDto);

        // Stand silme
        Task<bool> DeleteStandAsync(Guid standId);

        // Bir fuara ait tüm standları listeleme
        Task<List<StandDto>> GetStandsByFairAsync(Guid fairId);

        // Bir katılımcıya ait tüm standları listeleme
        Task<List<StandDto>> GetStandsByParticipantAsync(Guid participantId);

        // Stand ödeme durumu güncelleme
        Task<StandDto> UpdatePaymentStatusAsync(StandPaymentStatusDto paymentStatusDto);

        // Stand detaylarını alma
        Task<StandDto> GetStandByIdAsync(Guid standId);

        Task<List<StandDto>> GetStandsAsync(StandFilterDto filterDto);

        Task<List<Stand>> GetStandsWithUpcomingDueDateAsync(int days);
        Task<List<Stand>> GetUnpaidStandsAsync();

    }
}
