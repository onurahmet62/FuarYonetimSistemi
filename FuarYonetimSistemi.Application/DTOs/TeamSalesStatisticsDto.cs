using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class TeamSalesStatisticsDto
    {
        public Guid ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;

        // Takım genel istatistikleri
        public int TotalTeamMembers { get; set; }
        public int TotalTeamStandsSold { get; set; }
        public decimal TotalTeamRevenue { get; set; }
        public decimal TotalTeamPayments { get; set; }
        public decimal TeamPaymentCollectionRate { get; set; }

        // En iyi performans gösterenler
        public string TopPerformerName { get; set; } = string.Empty;
        public int TopPerformerStands { get; set; }
        public decimal TopPerformerRevenue { get; set; }

        // Takım üyelerinin detayları
        public List<SalesStatisticsDto> TeamMemberStatistics { get; set; } = new();
    }
}
