using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FuarYonetimSistemi.Domain.Enums;

namespace FuarYonetimSistemi.Domain.Entities
{
    public class Fair
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [ MaxLength(300)]
        public string Location { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Organizer { get; set; } = string.Empty;

        [Range(1900, 2100)]
        public int Year { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        public int StandCount { get; set; } // İstersen EF'de hesaplanan olarak ayarlayabiliriz

        
        public Guid CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        public ICollection<Stand> Stands { get; set; } = new List<Stand>();

        [MaxLength(100)]
        public string FairType { get; set; } = string.Empty;

        [MaxLength(250), Url]
        public string Website { get; set; } = string.Empty;

        [MaxLength(150), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int TotalParticipantCount { get; set; }

        [Range(0, int.MaxValue)]
        public int ForeignParticipantCount { get; set; }

        [Range(0, int.MaxValue)]
        public int TotalVisitorCount { get; set; }

        [Range(0, int.MaxValue)]
        public int ForeignVisitorCount { get; set; }

        [Range(0, double.MaxValue)]
        public double TotalStandArea { get; set; }

        [MaxLength(500)]
        public string ParticipatingCountries { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Budget { get; set; }

        [Range(0, double.MaxValue)]
        public decimal RevenueTarget { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ExpenseTarget { get; set; }

        [Range(0, double.MaxValue)]
        public decimal NetProfitTarget { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ActualRevenue { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ActualExpense { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ActualNetProfit { get; set; }

        public ICollection<FairExpense> FairExpenses { get; set; } = new List<FairExpense>();
    }
}
