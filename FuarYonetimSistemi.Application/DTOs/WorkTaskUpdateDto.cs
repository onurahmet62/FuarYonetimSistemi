using FuarYonetimSistemi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTaskStatus = FuarYonetimSistemi.Domain.Enums.WorkTaskStatus;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class WorkTaskUpdateDto
    {
        [Required]
        public Guid Id { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public WorkTaskStatus? Status { get; set; }

        public TaskPriority? Priority { get; set; }

        public DateTime? DueDate { get; set; }

        public Guid? AssignedToUserId { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
