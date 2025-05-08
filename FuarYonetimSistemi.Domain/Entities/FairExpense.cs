using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Domain.Entities
{
    public class FairExpense
    {
        public Guid Id { get; set; }

        public Guid FairId { get; set; }  // Fuarla ilişki
        public Fair Fair { get; set; }  // Fuar ile ilgili detaylar

        public Guid FairExpenseTypeId { get; set; }  // Gider türüyle ilişki
        public FairExpenseType ExpenseType { get; set; }

        public string AccountCode { get; set; }
        public decimal AnnualTarget { get; set; }
        public decimal AnnualActual { get; set; }
        public decimal CurrentTarget { get; set; }
        public decimal CurrentActual { get; set; }
        public decimal RealizedExpense { get; set; }  // Gerçekleşen gider

        public bool IsDeleted { get; set; } = false;
    }

}
