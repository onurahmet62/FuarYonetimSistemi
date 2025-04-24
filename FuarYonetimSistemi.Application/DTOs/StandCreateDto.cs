using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class StandCreateDto
    {
        public string Name { get; set; }
        public string FairHall { get; set; }
        public decimal Price { get; set; }
        public double Area { get; set; }  // Metrekare
        public Guid FairId { get; set; }  // Fuar ID'si
        public Guid ParticipantId { get; set; }  // Katılımcı ID'si
        public string Description { get; set; }

        public string PaymentStatus { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountRemaining { get; set; }
        public DateTime? DueDate { get; set; }
    }

}
