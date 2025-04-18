using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class FairCreateDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
      

        public Guid? CategoryId { get; set; } // Mevcut kategori seçimi için
        public string NewCategoryName { get; set; } // Yeni kategori eklemek için
    }

}
