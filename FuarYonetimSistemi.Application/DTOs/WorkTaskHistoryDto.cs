using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class WorkTaskHistoryDto
    {
        public Guid Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? Description { get; set; }
        public DateTime ChangedAt { get; set; }
        public Guid ChangedByUserId { get; set; }
        public string ChangedByUserName { get; set; } = string.Empty;
    }
}
