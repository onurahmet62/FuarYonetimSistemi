using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class FairExpenseFilterDto
    {
        public Guid? FairId { get; set; }
        public Guid? ExpenseTypeId { get; set; }
        public string AccountCode { get; set; }

        public decimal? MinAnnualTarget { get; set; }
        public decimal? MaxAnnualTarget { get; set; }

        public string SortBy { get; set; } = "AccountCode";
        public bool IsDescending { get; set; } = false;

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
