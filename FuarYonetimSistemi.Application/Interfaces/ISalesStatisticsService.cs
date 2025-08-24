using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Interfaces
{
    public interface ISalesStatisticsService
    {
        // Kullanıcı bazlı istatistikler
        Task<SalesStatisticsDto> GetUserSalesStatisticsAsync(Guid userId, SalesStatisticsRequestDto request);

        // Takım bazlı istatistikler (Manager'lar için)
        Task<TeamSalesStatisticsDto> GetTeamSalesStatisticsAsync(Guid managerId, SalesStatisticsRequestDto request);

        // Dashboard istatistikleri
        Task<DashboardStatisticsDto> GetDashboardStatisticsAsync(Guid currentUserId, SalesStatisticsRequestDto request);

        // Erişim yetkisi olan standları getir (hierarchical)
        Task<IEnumerable<Stand>> GetAccessibleStandsAsync(Guid userId);

        // Kullanıcının yönetebileceği kullanıcıları getir
        Task<IEnumerable<User>> GetManagedUsersAsync(Guid userId);
    }
}
