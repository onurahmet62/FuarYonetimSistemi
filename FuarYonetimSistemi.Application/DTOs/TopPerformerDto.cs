using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class TopPerformerDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int StandsSold { get; set; }
        public decimal Revenue { get; set; }
    }
}
