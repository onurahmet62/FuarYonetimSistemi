using FuarYonetimSistemi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class ParticipantDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime CreateDate { get; set; }
        public string AuthFullName { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }

        // These should be collections of DTOs, not entities
        public ICollection<BranchDto> Branches { get; set; }
        public ICollection<BrandDto> Brands { get; set; }
        public ICollection<ProductCategoryDto> ProductCategories { get; set; }
        public ICollection<ExhibitedProductDto> ExhibitedProducts { get; set; }
        public ICollection<RepresentativeCompanyDto> RepresentativeCompanies { get; set; }

        public string LogoFileName { get; set; }
        public string LogoFilePath { get; set; }
        public string LogoContentType { get; set; }
        public long LogoFileSize { get; set; }
        public DateTime? LogoUploadDate { get; set; }
        public string LogoUrl { get; set; }

        public List<string> FairNames { get; set; } = new();

    }
}
