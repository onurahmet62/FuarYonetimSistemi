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
        public decimal Price { get; set; }
        public double Area { get; set; }  // Metrekare
        public Guid FairId { get; set; }  // Fuar ID'si
        public Guid ParticipantId { get; set; }  // Katılımcı ID'si
        public string Description { get; set; }
    }

}
