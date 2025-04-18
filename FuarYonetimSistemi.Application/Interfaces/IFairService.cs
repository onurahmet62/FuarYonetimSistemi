using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FuarYonetimSistemi.Application.Interfaces
{
    public interface IFairService
    {
        Task<IEnumerable<FairDto>> GetAllFairsAsync();
        Task<FairDto> GetFairByIdAsync(Guid id);
        Task<FairDto> CreateFairAsync(FairCreateDto fairDto);
        Task<bool> DeleteFairAsync(Guid id);
    }

}
