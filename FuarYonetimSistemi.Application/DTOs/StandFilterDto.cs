using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class StandFilterDto
    {
        public Guid? FairId { get; set; }
        public Guid? ParticipantId { get; set; }
        public string? PaymentStatus { get; set; }
        public string? FairHall { get; set; }
        public string? Search { get; set; }
        public string? SortBy { get; set; } = "Name";  // varsayılan sıralama
        public bool SortDescending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public decimal AmountPaid { get; set; }
        public decimal AmountRemaining { get; set; }
        public DateTime? DueDate { get; set; }
    }

}
