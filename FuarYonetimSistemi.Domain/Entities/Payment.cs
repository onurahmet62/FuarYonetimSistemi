using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FuarYonetimSistemi.Domain.Entities
{
    public class Payment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Tutar negatif olamaz.")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty; // Havale, Peşin vb.

        [MaxLength(500)]
        public string PaymentDescription { get; set; } = string.Empty; // Peşinat, 2. Taksit vb.

        [Required]
        public Guid StandId { get; set; }

        [JsonIgnore]
        public Stand Stand { get; set; }

        public bool IsDeleted { get; set; } = false;

        [MaxLength(100)]
        public string ReceivedBy { get; set; } = string.Empty; // Kim teslim aldı
    }
}
