using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class SalesStatisticsRequestDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? FairId { get; set; }
        public Guid? UserId { get; set; } // Belirli bir kullanıcının istatistikleri için
        public bool IncludeTeamData { get; set; } = false; // Manager'lar için takım verilerini dahil et
    }
}
