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
        Task<IEnumerable<Stand>> GetAllAsync();  // Tüm standları getir
        Task<Stand> GetByIdAsync(Guid id);  // Id'ye göre stand getir
        Task<IEnumerable<Stand>> GetByFairIdAsync(Guid fairId);  // Fuar bazında standları getir
        Task<IEnumerable<Stand>> GetByParticipantIdAsync(Guid participantId);  // Katılımcıya ait standları getir
        Task<IEnumerable<Stand>> GetBySalesRepresentativeIdAsync(Guid salesRepId);  // Satış temsilcisine ait standları getir
        Task<Stand> AddAsync(StandCreateDto standCreateDto);  // Yeni bir stand ekle
        Task<Stand> UpdateAsync(Guid id, Stand updatedStand);  // Stand güncelle
        Task<bool> DeleteAsync(Guid id);  // Stand silme
        Task<IEnumerable<Stand>> FilterAsync(string searchTerm);  // Arama
        Task<IEnumerable<Stand>> GetSortedAsync(StandFilterRequestDto filter);  // Filtreleme ve sıralama
        Task<IEnumerable<Stand>> GetStandsDueInDaysAsync(int days);  // Vadesi yaklaşan standlar
    }
}
