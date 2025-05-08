using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class OfficeExpenseCreateDto
    {
        public Guid OfficeExpenseTypeId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}
