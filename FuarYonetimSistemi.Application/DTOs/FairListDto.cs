using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class FairListDto
    {
        public Guid Id { get; set; }  // Fuar ID'si
        public string Name { get; set; }  // Fuar adı
        public int Year { get; set; }

       
        public int MaxStandCount { get; set; }
        public DateTime StartDate { get; set; }  // Başlangıç tarihi
        public DateTime EndDate { get; set; }  // Bitiş tarihi
        public string Location { get; set; }  // Fuarın yapıldığı yer
    }
}
