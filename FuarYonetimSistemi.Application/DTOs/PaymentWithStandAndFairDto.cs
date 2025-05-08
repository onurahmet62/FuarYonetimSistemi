using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class PaymentWithStandAndFairDto
    {
        public Guid Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentDescription { get; set; }
        public Guid StandId { get; set; }
        public bool IsDeleted { get; set; }
        public string ReceivedBy { get; set; }

        public StandDto Stand { get; set; }
        public FairDto Fair { get; set; }
    }

}
