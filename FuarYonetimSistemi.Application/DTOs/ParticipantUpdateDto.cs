using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class ParticipantUpdateDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // Yeni eklenen firma bilgileri
        public string CompanyName { get; set; }  // Firma adı
        public string Address { get; set; }      // Firma adresi
       
        public string Website { get; set; }      // Firma web sitesi
        public string Branches { get; set; }     // Firma şubeleri

        public string AuthFullName { get; set; }
    }
}
