using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Domain.Entities
{
    public class OfficeExpense
    {
        public Guid Id { get; set; }
        public Guid OfficeExpenseTypeId { get; set; }
        public OfficeExpenseType ExpenseType { get; set; } = null!; // Gider Türü ilişkisi

        [Required]
        public DateTime Date { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; } // Miktar

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty; // Açıklama

        public bool IsDeleted { get; set; } = false;
    }


}
