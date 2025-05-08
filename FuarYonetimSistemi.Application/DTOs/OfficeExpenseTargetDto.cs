using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class OfficeExpenseTargetDto
    {
        public Guid Id { get; set; }
        public Guid ExpenseTypeId { get; set; }
        public string FairName { get; set; }
        public int Year { get; set; }
        public decimal TargetAmount { get; set; }
    }

}
