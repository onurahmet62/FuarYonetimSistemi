using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuarYonetimSistemi.Domain.Entities
{
    public class Fair
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public string Name { get; set; } = null!;  // Zorunlu, boş bırakılamaz

        [MaxLength(300)]
        public string? Location { get; set; }  // Opsiyonel, nullable

        [MaxLength(200)]
        public string? Organizer { get; set; }  // Opsiyonel, nullable

        [Range(1900, 2100)]
        public int? Year { get; set; }  // Opsiyonel, nullable

        [Required]
        public DateTime StartDate { get; set; }  // Zorunlu

        [Required]
        public DateTime EndDate { get; set; }  // Zorunlu

        public bool IsDeleted { get; set; } = false;

        public int? StandCount { get; set; }  // Opsiyonel, nullable

        public Guid? CategoryId { get; set; }  // Opsiyonel, nullable

        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }  // Opsiyonel, nullable

        public ICollection<Stand>? Stands { get; set; }  // Opsiyonel, nullable

        [MaxLength(100)]
        public string? FairType { get; set; }  // Opsiyonel, nullable

        [MaxLength(250), Url]
        public string? Website { get; set; }  // Opsiyonel, nullable

        [MaxLength(150), EmailAddress]
        public string? Email { get; set; }  // Opsiyonel, nullable

        [Range(0, int.MaxValue)]
        public int? TotalParticipantCount { get; set; }  // Opsiyonel, nullable

        [Range(0, int.MaxValue)]
        public int? ForeignParticipantCount { get; set; }  // Opsiyonel, nullable

        [Range(0, int.MaxValue)]
        public int? TotalVisitorCount { get; set; }  // Opsiyonel, nullable

        [Range(0, int.MaxValue)]
        public int? ForeignVisitorCount { get; set; }  // Opsiyonel, nullable

        [Range(0, double.MaxValue)]
        public double? TotalStandArea { get; set; }  // Opsiyonel, nullable

        [MaxLength(500)]
        public string? ParticipatingCountries { get; set; }  // Opsiyonel, nullable

        [Range(0, double.MaxValue)]
        public decimal? Budget { get; set; }  // Opsiyonel, nullable

        [Range(0, double.MaxValue)]
        public decimal? RevenueTarget { get; set; }  // Opsiyonel, nullable

        [Range(0, double.MaxValue)]
        public decimal? ExpenseTarget { get; set; }  // Opsiyonel, nullable

        [Range(0, double.MaxValue)]
        public decimal? NetProfitTarget { get; set; }  // Opsiyonel, nullable

        [Range(0, double.MaxValue)]
        public decimal? ActualRevenue { get; set; }  // Opsiyonel, nullable

        [Range(0, double.MaxValue)]
        public decimal? ActualExpense { get; set; }  // Opsiyonel, nullable

        [Range(0, double.MaxValue)]
        public decimal? ActualNetProfit { get; set; }  // Opsiyonel, nullable

        public ICollection<FairExpense>? FairExpenses { get; set; }  // Opsiyonel, nullable
    }
}
