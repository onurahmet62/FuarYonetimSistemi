using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } // Havale, Peşin vb.

        public string PaymentDescription { get; set; } // Açıklama (Örneğin: Peşinat, 2. Taksit vb.)

        public Guid StandId { get; set; }
        public Stand Stand { get; set; }

  
        public string ReceivedBy { get; set; } // Kim teslim aldı (User yerine basit string)
    }
}
