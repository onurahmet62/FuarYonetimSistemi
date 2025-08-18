using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class BranchDto
    {
        public int Id { get; set; } // ✅ Id alanı eklendi
        public string Name { get; set; } = string.Empty;
    }
}
