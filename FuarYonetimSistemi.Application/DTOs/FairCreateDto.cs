using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class FairCreateDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Organizer { get; set; }
        public Guid? CategoryId { get; set; } // Mevcut kategori seçimi için
        public string NewCategoryName { get; set; } // Yeni kategori eklemek için

        // Yeni Alanlar:
        public string FairType { get; set; }  // Fuar türü (İhtisas, Genel, vb.)
        public string Website { get; set; } // Web adresi
        public string Email { get; set; } // E-posta adresi
        public int TotalParticipantCount { get; set; } // Katılımcı firma sayısı
        public int ForeignParticipantCount { get; set; } // Yabancı katılımcı sayısı
        public int TotalVisitorCount { get; set; } // Toplam ziyaretçi sayısı
        public int ForeignVisitorCount { get; set; } // Yabancı ziyaretçi sayısı
        public double TotalStandArea { get; set; } // Kurulan standların toplam alanı
        public string ParticipatingCountries { get; set; } // Katılan ülkeler
        public decimal Budget { get; set; } // Fuarın bütçesi


        // Gelir ve Gider ile ilgili yeni alanlar:
        public decimal RevenueTarget { get; set; } // Gelir hedefi
        public decimal ExpenseTarget { get; set; } // Gider hedefi
        public decimal NetProfitTarget { get; set; } // Net kar hedefi

        public decimal ActualRevenue { get; set; } // Gerçekleşen gelir
        public decimal ActualExpense { get; set; } // Gerçekleşen gider
        public decimal ActualNetProfit { get; set; } // Gerçekleşen net kar
    }

}
