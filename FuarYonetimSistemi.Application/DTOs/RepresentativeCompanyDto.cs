using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class RepresentativeCompanyDto
    {
        public int Id { get; set; } // ✅ Id alanı eklendi
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
    }

}
