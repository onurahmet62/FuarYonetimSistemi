using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class DashboardStatisticsDto
    {
        // Genel sistem istatistikleri
        public int TotalActiveStands { get; set; }
        public decimal TotalSystemRevenue { get; set; }
        public decimal TotalOutstandingBalance { get; set; }
        public int TotalActiveSalesReps { get; set; }

        // Bu ay/yıl karşılaştırmaları
        public int StandsThisMonth { get; set; }
        public int StandsLastMonth { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public decimal RevenueLastMonth { get; set; }

        // Performans metrikleri
        public decimal AverageStandValue { get; set; }
        public decimal SystemPaymentCollectionRate { get; set; }

        // Uyarılar ve dikkat edilmesi gerekenler
        public int OverdueStandsCount { get; set; }
        public int StandsDueSoonCount { get; set; }
        public decimal OverdueAmount { get; set; }

        // Top performans
        public List<TopPerformerDto> TopSalesReps { get; set; } = new();
        public List<FairPerformanceDto> TopFairs { get; set; } = new();
    }
}
