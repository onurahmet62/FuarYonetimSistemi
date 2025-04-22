using FuarYonetimSistemi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Domain.Entities
{
    public class Fair
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Organizer { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsDeleted { get; set; }  // Silinmiş kontrolü için
        public int StandCount { get; set; }
        public Guid CategoryId { get; set; } // Foreign key
        public Category Category { get; set; } // Navigation property
        public ICollection<Stand> Stands { get; set; }  // Fuarın tüm standları
       

       
    }

}
