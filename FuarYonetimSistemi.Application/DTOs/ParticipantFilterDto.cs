using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class ParticipantFilterDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
      
        public string? AuthFullName { get; set; }

        // Yeni eklenen filtreleme alanları
        public string? CompanyName { get; set; }  // Firma adı
       

        public DateTime? CreateDate { get; set; } // Tam eşleşme yapar
        public bool? HasLogo { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Sıralama
        public string? SortBy { get; set; } = "CreateDate"; // default
        public bool IsDescending { get; set; } = true; // default azalan
    }
}
