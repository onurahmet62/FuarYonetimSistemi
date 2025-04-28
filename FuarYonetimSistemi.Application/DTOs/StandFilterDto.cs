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
        public string PaymentStatus { get; set; }
        public string Search { get; set; }
        public string SortBy { get; set; } = "name";
        public bool SortDescending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

}
