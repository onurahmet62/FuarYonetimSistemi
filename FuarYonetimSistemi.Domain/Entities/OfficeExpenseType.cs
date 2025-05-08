using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Domain.Entities
{
    public class OfficeExpenseType
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
    }

}
