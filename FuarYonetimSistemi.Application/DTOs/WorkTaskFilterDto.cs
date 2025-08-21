using FuarYonetimSistemi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTaskStatus = FuarYonetimSistemi.Domain.Enums.WorkTaskStatus;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class WorkTaskFilterDto
    {
        public WorkTaskStatus? Status { get; set; }
        public TaskPriority? Priority { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public string? SearchTerm { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        // Sorting
        public string SortBy { get; set; } = "CreatedAt";
        public bool IsDescending { get; set; } = true;
    }
}
