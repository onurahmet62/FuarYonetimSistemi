using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class StandSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FairHall { get; set; } = string.Empty;
        public string ParticipantName { get; set; } = string.Empty;
        public string FairName { get; set; } = string.Empty;
        public string SalesRepresentativeName { get; set; } = string.Empty;
        public decimal? TotalAmountWithVAT { get; set; }
        public decimal? Balance { get; set; }
        public DateTime? ContractDate { get; set; }
        public DateTime? ActualDueDate { get; set; }
        public bool IsOverdue { get; set; }
        public int? DaysUntilDue { get; set; }
    }

}
