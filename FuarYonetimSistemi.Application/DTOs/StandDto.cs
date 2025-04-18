using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class StandDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public double Area { get; set; }
        public string Description { get; set; }
        public string ParticipantFullName { get; set; }  // Katılımcının adı
        public string FairName { get; set; }  // Fuar adı
        public string PaymentStatus { get; set; }  // Ödeme durumu (Ödeme Alınmadı, Ödeme Tamamlandı, vb.)
        public decimal AmountPaid { get; set; }  // Ödenen miktar
        public decimal AmountRemaining { get; set; }  // Kalan ödeme
        public DateTime? DueDate { get; set; }  // Son ödeme tarihi (varsa)
    }

}
