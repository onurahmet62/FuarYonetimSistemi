using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class FairDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;  // Zorunlu, boş olamaz

        public string? Location { get; set; }
        public int? Year { get; set; }
        public string? Organizer { get; set; }

        public DateTime StartDate { get; set; }  // Zorunlu
        public DateTime EndDate { get; set; }  // Zorunlu

        public string? CategoryName { get; set; }

        // Yeni Alanlar:
        public string? FairType { get; set; }
        public string? Website { get; set; }
        public string? Email { get; set; }
        public int? TotalParticipantCount { get; set; }
        public int? ForeignParticipantCount { get; set; }
        public int? TotalVisitorCount { get; set; }
        public int? ForeignVisitorCount { get; set; }
        public double? TotalStandArea { get; set; }
        public string? ParticipatingCountries { get; set; }
        public decimal? Budget { get; set; }

        // Gelir ve Gider ile ilgili yeni alanlar:
        public decimal? RevenueTarget { get; set; }
        public decimal? ExpenseTarget { get; set; }
        public decimal? NetProfitTarget { get; set; }

        public decimal? ActualRevenue { get; set; }
        public decimal? ActualExpense { get; set; }
        public decimal? ActualNetProfit { get; set; }
    }
}
