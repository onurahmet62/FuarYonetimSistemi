using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class PaymentFilterDto
    {
        public string? PaymentMethod { get; set; }
        public string? ReceivedBy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SortBy { get; set; } // "PaymentDate", "Amount", etc.
        public bool SortDescending { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public Guid? ParticipantId { get; set; } // ✅ Yeni eklendi
    }

}
