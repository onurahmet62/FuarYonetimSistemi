using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class StandUpdateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string FairHall { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public double Area { get; set; }
        public string PaymentStatus { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountRemaining { get; set; }
        public DateTime? DueDate { get; set; }
    }

}
