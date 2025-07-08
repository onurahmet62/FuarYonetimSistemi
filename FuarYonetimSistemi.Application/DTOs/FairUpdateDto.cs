using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class FairUpdateDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(300)]
        public string? Location { get; set; }

        [MaxLength(200)]
        public string? Organizer { get; set; }

        [Range(1900, 2100)]
        public int? Year { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public Guid? CategoryId { get; set; }

        public string? NewCategoryName { get; set; }

        public string? FairType { get; set; }

        [MaxLength(250)]
        public string? Website { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        public int? TotalParticipantCount { get; set; }
        public int? ForeignParticipantCount { get; set; }
        public int? TotalVisitorCount { get; set; }
        public int? ForeignVisitorCount { get; set; }

        public double? TotalStandArea { get; set; }

        public string? ParticipatingCountries { get; set; }

        public decimal? Budget { get; set; }

        public decimal? RevenueTarget { get; set; }
        public decimal? ExpenseTarget { get; set; }
        public decimal? NetProfitTarget { get; set; }

        public decimal? ActualRevenue { get; set; }
        public decimal? ActualExpense { get; set; }
        public decimal? ActualNetProfit { get; set; }
    }
}
