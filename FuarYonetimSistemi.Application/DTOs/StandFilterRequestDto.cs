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
        public string? SalesRepresentative { get; set; }

        public string? SortBy { get; set; }
        public bool IsDescending { get; set; } = false;

        private int _pageNumber = 1;
        public int PageNumber
        {
            get => _pageNumber <= 0 ? 1 : _pageNumber;
            set => _pageNumber = value;
        }

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize <= 0 ? 10 : _pageSize;
            set => _pageSize = value;
        }
    }


}
