using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class StandPaymentStatusDto
    {
        public Guid StandId { get; set; }
        public string PaymentStatus { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountRemaining { get; set; }
        public DateTime? DueDate { get; set; }
    }

}
