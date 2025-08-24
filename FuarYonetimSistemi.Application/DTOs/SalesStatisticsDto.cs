using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class SalesStatisticsDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;

        // Stand satış istatistikleri
        public int TotalStandsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalWithVAT { get; set; }
        public decimal TotalPaymentsReceived { get; set; }
        public decimal TotalOutstandingBalance { get; set; }

        // Performans metrikleri
        public decimal AverageStandValue { get; set; }
        public double AverageStandArea { get; set; }
        public decimal PaymentCollectionRate { get; set; } // Tahsilat oranı

        // Zamana dayalı veriler
        public int StandsSoldThisMonth { get; set; }
        public int StandsSoldThisYear { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public decimal RevenueThisYear { get; set; }

        // Vadesi geçen/yaklaşan
        public int OverdueStands { get; set; }
        public int StandsDueSoon { get; set; } // 30 gün içinde vadesi gelenler
        public decimal OverdueAmount { get; set; }
    }
}
