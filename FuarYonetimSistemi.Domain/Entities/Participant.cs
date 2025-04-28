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

        // Yeni eklenen firma bilgileri
        public string CompanyName { get; set; }  // Firma adı
        public string Address { get; set; }      // Firma adresi
        public string PhoneNumbers { get; set; } // Firma telefon numaraları
        public string Website { get; set; }      // Firma web sitesi
        public string Branches { get; set; }     // Firma şubeleri

        public bool IsDeleted { get; set; }  // Silinmiş kontrolü için
        public DateTime CreateDate { get; set; }
        public string AuthFullName { get; set; }

        public ICollection<Stand> Stands { get; set; } = new List<Stand>();

    }

}
