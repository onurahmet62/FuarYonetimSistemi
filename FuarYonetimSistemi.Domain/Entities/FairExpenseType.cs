using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Domain.Entities
{
    public class FairExpenseType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false;

        public ICollection<FairExpense> FairExpenses { get; set; }
    }

}
