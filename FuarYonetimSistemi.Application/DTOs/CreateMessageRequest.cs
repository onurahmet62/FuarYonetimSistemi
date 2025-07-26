using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class CreateMessageRequest
    {
        public Guid ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
