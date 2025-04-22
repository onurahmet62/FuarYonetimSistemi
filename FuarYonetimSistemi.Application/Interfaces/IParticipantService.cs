using FuarYonetimSistemi.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Interfaces
{
    // IParticipantService.cs
    public interface IParticipantService
    {
        Task<ParticipantDto> CreateAsync(CreateParticipantDto dto);
        Task<ParticipantDto> UpdateAsync(Guid id, UpdateParticipantDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<ParticipantDto> GetByIdAsync(Guid id);
        Task<IEnumerable<ParticipantDto>> GetAllAsync();
     
        Task<PagedResult<ParticipantDto>> FilterPagedAsync(ParticipantFilterDto filter);
    }

}
