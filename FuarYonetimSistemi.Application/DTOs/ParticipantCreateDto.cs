using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class ParticipantCreateDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
     

        public DateTime CreateDate { get; set; }
        public string AuthFullName { get; set; }
    }
}
