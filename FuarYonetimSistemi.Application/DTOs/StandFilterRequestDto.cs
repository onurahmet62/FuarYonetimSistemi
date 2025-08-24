using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class StandFilterRequestDto
    {
        public string? Name { get; set; }
        public string? FairHall { get; set; }
        public DateTime? ContractDate { get; set; }
        public DateTime? ActualDueDate { get; set; }
        public Guid? ParticipantId { get; set; }
        public Guid? FairId { get; set; }

        // Sales Representative filtreleri
        public Guid? SalesRepresentativeId { get; set; }
        public string? SalesRepresentativeName { get; set; }

        // Sorting
        public string? SortBy { get; set; }
        public bool IsDescending { get; set; } = false;

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }


}
