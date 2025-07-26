using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Interfaces
{
    public interface ISharedFileService
    {

        Task<IEnumerable<SharedFile>> GetAllFilesAsync();
        Task<SharedFile> GetFileByIdAsync(Guid id);
        Task<SharedFile> CreateFileAsync(SharedFileCreateDto dto);
        Task<bool> UpdateFileAsync(Guid id, SharedFileCreateDto dto);
        Task<bool> DeleteFileAsync(Guid id);
    }

}
