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
        public string Name { get; set; }
        public string ParticipantFullName { get; set; }
        public string PaymentStatus { get; set; }
    }

}
