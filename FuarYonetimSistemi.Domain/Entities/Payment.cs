using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }

        // Stand ile ilişki
        public Guid StandId { get; set; }
        public Stand Stand { get; set; }

        // Either use this string field
        public string ReceivedBy { get; set; }

        public Guid ParticipantId { get; set; }
        public Participant Participant { get; set; }

        // OR use a proper relationship to User (but not both)
        // public Guid ReceivedByUserId { get; set; }
        // public User ReceivedByUser { get; set; }
    }
}
