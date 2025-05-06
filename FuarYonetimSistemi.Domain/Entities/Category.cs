using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FuarYonetimSistemi.Domain.Entities
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Fair> Fairs { get; set; } = new List<Fair>(); // Navigation property
    }
}
