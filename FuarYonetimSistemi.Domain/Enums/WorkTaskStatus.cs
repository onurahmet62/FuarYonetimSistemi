using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Domain.Enums
{
    public enum WorkTaskStatus
    {
        Planned = 0,        // Planlandı
        InProgress = 1,     // Yapılıyor
        Completed = 2,      // Yapıldı
        Cancelled = 3       // İptal Edildi
    }
}
