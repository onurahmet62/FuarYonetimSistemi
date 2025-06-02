using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class UpdateParticipantDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // Yeni eklenen firma bilgileri
        public string CompanyName { get; set; }  // Firma adı
        public string Address { get; set; }      // Firma adresi
      // Firma telefon numaraları
        public string Website { get; set; }      // Firma web sitesi
        public string Branches { get; set; }     // Firma şubeleri

        public DateTime CreateDate { get; set; }
        public string AuthFullName { get; set; }

        // Logo dosyası (opsiyonel - null ise mevcut logo korunur)
        public IFormFile? LogoFile { get; set; }

        // Logo silme kontrolü
        public bool RemoveLogo { get; set; } = false;
    }

}
