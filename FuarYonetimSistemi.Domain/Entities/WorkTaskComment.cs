using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Domain.Entities
{
    public class WorkTaskComment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid WorkTaskId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid CreatedByUserId { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        [ForeignKey(nameof(WorkTaskId))]
        public WorkTask WorkTask { get; set; } = null!;

        [ForeignKey(nameof(CreatedByUserId))]
        public User CreatedByUser { get; set; } = null!;
    }
}