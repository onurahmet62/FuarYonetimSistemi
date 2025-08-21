using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class WorkTaskAssignmentUpdateDto
    {
        [Required]
        public Guid WorkTaskId { get; set; }

        [Required]
        public Guid NewAssignedToUserId { get; set; }

        [MaxLength(1000)]
        public string? AssignmentComment { get; set; }
    }
}
