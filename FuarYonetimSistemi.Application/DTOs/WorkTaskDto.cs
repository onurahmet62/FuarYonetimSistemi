using FuarYonetimSistemi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTaskStatus = FuarYonetimSistemi.Domain.Enums.WorkTaskStatus;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class WorkTaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public WorkTaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Notes { get; set; }

        // User Information
        public Guid CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public Guid AssignedToUserId { get; set; }
        public string AssignedToUserName { get; set; } = string.Empty;

        // Related Data
        public List<WorkTaskHistoryDto>? WorkTaskHistories { get; set; }
        public List<WorkTaskCommentDto>? WorkTaskComments { get; set; }
    }
}
