using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Domain.Entities
{
    public class WorkTaskHistory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid WorkTaskId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Action { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? OldValue { get; set; }

        [MaxLength(500)]
        public string? NewValue { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid ChangedByUserId { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(WorkTaskId))]
        public WorkTask WorkTask { get; set; } = null!;

        [ForeignKey(nameof(ChangedByUserId))]
        public User ChangedByUser { get; set; } = null!;
    }
}
