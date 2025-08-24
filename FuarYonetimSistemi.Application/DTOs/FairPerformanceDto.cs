using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class FairPerformanceDto
    {
        public Guid FairId { get; set; }
        public string FairName { get; set; } = string.Empty;
        public int StandsSold { get; set; }
        public decimal Revenue { get; set; }
        public int ParticipantCount { get; set; }
    }
}
