using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class PaymentDetailDto
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentDescription { get; set; }
        public string ReceivedBy { get; set; }

        public Guid StandId { get; set; }
        public string StandName { get; set; }
        public decimal StandArea { get; set; }

        public Guid ParticipantId { get; set; }
        public string ParticipantName { get; set; }

        public Guid FairId { get; set; }
        public string FairName { get; set; }
        public DateTime FairStartDate { get; set; }
    }

}
