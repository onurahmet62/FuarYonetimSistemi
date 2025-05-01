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
        public string? Organizer { get; set; }
        public string? Location { get; set; }
        public int? Year { get; set; }
        public Guid? CategoryId { get; set; }
        public string? FairType { get; set; } // Enum ise
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

    }
}
