using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class FairFilterDto
    {
        public string? Name { get; set; }
        public string? Location { get; set; }
        public Guid? CategoryId { get; set; }
        public int? Year { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public string? SortBy { get; set; } // örn: "Name", "Year"
        public bool SortDescending { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
