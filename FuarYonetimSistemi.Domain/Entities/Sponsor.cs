using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Domain.Entities
{
    public class Sponsor
    {
        public Guid Id { get; set; }
        public string Name { get; set; }  // Sponsorun adı
        public string? Description { get; set; } // Sponsorun açıklaması
        public string? Website { get; set; } // Sponsorun web sitesi
        public ICollection<Fair> Fairs { get; set; } = new List<Fair>(); // Fuarlarla ilişkisi
    }
}
