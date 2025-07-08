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

        [Required, MaxLength(50)]
        public string Phone { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Website { get; set; } = string.Empty;

        [MaxLength(150)]
        public string AuthFullName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? LogoFileName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? LogoFilePath { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? LogoContentType { get; set; } = string.Empty;

        public long LogoFileSize { get; set; } = 0;

        public DateTime? LogoUploadDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        // 🔗 Navigation Properties
        public ICollection<Branch> Branches { get; set; } = new List<Branch>();
        public ICollection<Brand> Brands { get; set; } = new List<Brand>();
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
        public ICollection<RepresentativeCompany> RepresentativeCompanies { get; set; } = new List<RepresentativeCompany>();
        public ICollection<ExhibitedProduct> ExhibitedProducts { get; set; } = new List<ExhibitedProduct>();
        public ICollection<Stand> Stands { get; set; } = new List<Stand>();
    }

}
