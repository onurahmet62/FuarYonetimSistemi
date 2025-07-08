using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class UpdateParticipantDto
    {
        // Temel alanlar
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string AuthFullName { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }

        // Logo işlemleri için
        public bool RemoveLogo { get; set; }
        public IFormFile LogoFile { get; set; }

        // Mutlaka koleksiyon tipleri şöyle olmalı:
        public ICollection<BranchDto> Branches { get; set; } = new List<BranchDto>();
        public ICollection<BrandDto> Brands { get; set; } = new List<BrandDto>();
        public ICollection<ProductCategoryDto> ProductCategories { get; set; } = new List<ProductCategoryDto>();
        public ICollection<ExhibitedProductDto> ExhibitedProducts { get; set; } = new List<ExhibitedProductDto>();
        public ICollection<RepresentativeCompanyDto> RepresentativeCompanies { get; set; } = new List<RepresentativeCompanyDto>();
    }


}
