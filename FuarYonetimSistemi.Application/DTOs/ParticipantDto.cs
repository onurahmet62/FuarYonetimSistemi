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

        // ✅ Entity ile tam uyumlu temel alanlar
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string AuthFullName { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }

        // ✅ Logo alanları - Entity ile tam uyumlu
        public string? LogoFileName { get; set; }
        public string? LogoFilePath { get; set; }
        public string? LogoContentType { get; set; }
        public long LogoFileSize { get; set; }
        public DateTime? LogoUploadDate { get; set; }

        // ✅ Navigation Properties - Entity ile uyumlu
        public List<BranchDto>? Branches { get; set; }
        public List<BrandDto>? Brands { get; set; }
        public List<ProductCategoryDto>? ProductCategories { get; set; }
        public List<ExhibitedProductDto>? ExhibitedProducts { get; set; }
        public List<RepresentativeCompanyDto>? RepresentativeCompanies { get; set; }

        // ⚠️ Bu alanlar Entity'de yok - ihtiyaç varsa Entity'e eklenebilir
        // public List<StandDto>? Stands { get; set; } // Entity'de var ama şu an kullanılmıyor

        // 🔧 Computed/Helper alanlar (Entity'de olmayan ama API'de faydalı)
        public string? LogoUrl { get; set; } // API'de logo URL'si için
        public bool HasLogo => !string.IsNullOrEmpty(LogoFilePath);
        public int TotalBranches => Branches?.Count ?? 0;
        public int TotalBrands => Brands?.Count ?? 0;
        public int TotalProducts => ExhibitedProducts?.Count ?? 0;
        public int TotalRepresentatives => RepresentativeCompanies?.Count ?? 0;

    }
}
