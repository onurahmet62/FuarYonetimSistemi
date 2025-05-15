using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Interfaces
{
    public interface IUserService
    {
        Task<User> AddUserAsync(UserDto dto);
        Task<User> UpdateUserAsync(Guid id, UserDto dto);
        Task SoftDeleteUserAsync(Guid id);
        Task<User?> GetUserByIdAsync(Guid id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<string> ResetPasswordAsync(Guid id);
    }
}
