using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class StandDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FairHall { get; set; } = string.Empty;

        // Area Information
        public double? AreaSold { get; set; }
        public double? AreaExchange { get; set; }
        public double? ContractArea { get; set; }

        // Financial Information
        public decimal? UnitPrice { get; set; }
        public decimal? SaleAmountWithoutVAT { get; set; }
        public decimal? TotalAmountWithVAT { get; set; }
        public decimal? Balance { get; set; }
        public decimal? TotalPaymentsReceived { get; set; }

        // Dates
        public DateTime? ContractDate { get; set; }
        public DateTime? ActualDueDate { get; set; }

        // Related Entities
        public string ParticipantName { get; set; } = string.Empty;
        public string FairName { get; set; } = string.Empty;
        public string SalesRepresentativeName { get; set; } = string.Empty;
        public Guid? SalesRepresentativeId { get; set; }

        // Status Information
        public bool IsOverdue { get; set; }
        public int? DaysUntilDue { get; set; }
        public decimal PaymentCollectionRate { get; set; }

        public string Note { get; set; } = string.Empty;

        // Payment History
        public List<PaymentSummaryDto> PaymentHistory { get; set; } = new();
    }
}
