using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class CreateOfficeExpenseRequest
    {
        public Guid ExpenseTypeId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
    }

}
