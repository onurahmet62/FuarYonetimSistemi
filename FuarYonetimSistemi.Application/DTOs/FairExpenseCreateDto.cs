using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class FairExpenseCreateDto
    {
        public Guid FairId { get; set; }
        public Guid FairExpenseTypeId { get; set; }
        public string AccountCode { get; set; }
        public decimal AnnualTarget { get; set; }
        public decimal AnnualActual { get; set; }
        public decimal CurrentTarget { get; set; }
        public decimal CurrentActual { get; set; }
        public decimal RealizedExpense { get; set; }
    }
}
