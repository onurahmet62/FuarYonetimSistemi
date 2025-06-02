using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class ParticipantDto
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

        // Logo bilgileri
        public string? LogoFileName { get; set; }
        public string? LogoFilePath { get; set; }
        public string? LogoContentType { get; set; }
        public long LogoFileSize { get; set; }
        public DateTime? LogoUploadDate { get; set; }
        public string? LogoUrl { get; set; } // API üzerinden erişim için
        public byte[] LogoBytes { get; set; }

        public DateTime CreateDate { get; set; }
        public string AuthFullName { get; set; }
    }
}
