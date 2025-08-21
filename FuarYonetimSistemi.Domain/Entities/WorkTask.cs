using FuarYonetimSistemi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTaskStatus = FuarYonetimSistemi.Domain.Enums.WorkTaskStatus;

namespace FuarYonetimSistemi.Domain.Entities
{
    public class WorkTask
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public WorkTaskStatus Status { get; set; } = WorkTaskStatus.Planned;

        [Required]
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DueDate { get; set; }

        public DateTime? CompletedAt { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Foreign Keys
        [Required]
        public Guid CreatedByUserId { get; set; }  // Manager kim açtı

        [Required]
        public Guid AssignedToUserId { get; set; }  // SalesPerson kime atandı

        // Navigation Properties
        [ForeignKey(nameof(CreatedByUserId))]
        public User CreatedByUser { get; set; } = null!;

        [ForeignKey(nameof(AssignedToUserId))]
        public User AssignedToUser { get; set; } = null!;

        // Task History
        public ICollection<WorkTaskHistory> WorkTaskHistories { get; set; } = new List<WorkTaskHistory>();

        // Task Comments
        public ICollection<WorkTaskComment> WorkTaskComments { get; set; } = new List<WorkTaskComment>();
    }
}
