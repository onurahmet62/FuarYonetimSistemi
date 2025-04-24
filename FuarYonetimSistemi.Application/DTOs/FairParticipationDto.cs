using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class FairParticipationDto
    {
        public Guid FairId { get; set; }
        public string FairName { get; set; }
        public string Location { get; set; }
        public string FairHall { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Organizer { get; set; }
        public string CategoryName { get; set; }
    }
}
