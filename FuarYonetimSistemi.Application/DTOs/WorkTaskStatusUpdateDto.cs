using FuarYonetimSistemi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class WorkTaskStatusUpdateDto
    {
        [Required]
        public Guid WorkTaskId { get; set; }

        [Required]
        public WorkTaskStatus Status { get; set; }

        [MaxLength(1000)]
        public string? StatusChangeComment { get; set; }
    }
}
