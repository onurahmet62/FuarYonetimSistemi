using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class FairResponseDto
    {
        public Guid Id { get; set; }  // Fuar ID'si
        public string Name { get; set; }  // Fuar adı
        public DateTime StartDate { get; set; }  // Başlangıç tarihi
        public DateTime EndDate { get; set; }  // Bitiş tarihi
        public bool IsDeleted { get; set; }  // Fuar silinmiş mi?
       
    }
}
