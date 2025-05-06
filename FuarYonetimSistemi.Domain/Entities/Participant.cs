using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuarYonetimSistemi.Domain.Entities
{
    public class Participant
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(150), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(20), Phone]
        public string Phone { get; set; } = string.Empty;

        // Firma Bilgileri
        [Required, MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(100)]
        public string PhoneNumbers { get; set; } = string.Empty;

        [MaxLength(250), Url]
        public string Website { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Branches { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;

        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [MaxLength(150)]
        public string AuthFullName { get; set; } = string.Empty; // Yetkili kişi adı

        // Navigation Property
        public ICollection<Stand> Stands { get; set; } = new List<Stand>();
    }
}
