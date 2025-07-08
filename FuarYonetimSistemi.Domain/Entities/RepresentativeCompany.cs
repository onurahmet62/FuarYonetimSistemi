using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Domain.Entities
{
    public class RepresentativeCompany
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required, MaxLength(100)]
        public string Country { get; set; }

        [MaxLength(300)]
        public string Address { get; set; }

        [MaxLength(100)]
        public string District { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(50)]
        public string Phone { get; set; }

        [MaxLength(150), EmailAddress]
        public string Email { get; set; }

        [MaxLength(200)]
        public string Website { get; set; }

        [ForeignKey("Participant")]
        public Guid ParticipantId { get; set; }

        public Participant Participant { get; set; }
    }

}
