using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Domain.Entities
{
    public class Participant
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }  // Katılımcının adı
        public string Email { get; set; }     // Katılımcının e-posta adresi
        public string Phone { get; set; }     // Katılımcının telefon numarası

        public bool IsDeleted { get; set; }  // Silinmiş kontrolü için

        public DateTime CreateDate { get; set; }
        public string AuthFullName { get; set; }

        public ICollection<Stand> Stands { get; set; } = new List<Stand>();

    }

}
