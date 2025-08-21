using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class BulkWorkTaskStatusUpdateDto
    {
        [Required]
        public List<Guid> WorkTaskIds { get; set; } = new List<Guid>();

        [Required]
        public TaskStatus Status { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }
}
