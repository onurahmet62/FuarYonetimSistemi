using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class UpdateParticipantDto
    {
        // Temel alanlar
        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [StringLength(150, ErrorMessage = "Ad Soyad en fazla 150 karakter olabilir")]
        [SwaggerSchema(Description = "Katılımcının ad soyad bilgisi")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email adresi zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        [StringLength(150, ErrorMessage = "Email en fazla 150 karakter olabilir")]
        [SwaggerSchema(Description = "Katılımcının email adresi")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon numarası zorunludur")]
        [StringLength(50, ErrorMessage = "Telefon en fazla 50 karakter olabilir")]
        [SwaggerSchema(Description = "Katılımcının telefon numarası")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(150, ErrorMessage = "Yetkili kişi adı en fazla 150 karakter olabilir")]
        [SwaggerSchema(Description = "Yetkili kişinin ad soyad bilgisi")]
        public string? AuthFullName { get; set; }

        [Required(ErrorMessage = "Firma adı zorunludur")]
        [StringLength(200, ErrorMessage = "Firma adı en fazla 200 karakter olabilir")]
        [SwaggerSchema(Description = "Firmanın adı")]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Adres en fazla 300 karakter olabilir")]
        [SwaggerSchema(Description = "Firma adresi")]
        public string? Address { get; set; }

        [StringLength(250, ErrorMessage = "Website adresi en fazla 250 karakter olabilir")]
        [SwaggerSchema(Description = "Firma website adresi (örn: https://www.example.com veya www.example.com)")]
        public string? Website { get; set; }

        // Logo işlemleri için
        [SwaggerSchema(Description = "Logo dosyasını kaldır (true/false)")]
        public bool RemoveLogo { get; set; }

        [SwaggerSchema(Description = "Yeni logo dosyası (max 1MB, 500x300px)")]
        public IFormFile? LogoFile { get; set; }

        // ✅ JSON string alanları eklendi (multipart/form-data için gerekli)
        [SwaggerSchema(Description = "Şubeler JSON formatında: [{'name': 'Şube Adı'}]")]
        public string? BranchesJson { get; set; }

        [SwaggerSchema(Description = "Markalar JSON formatında: [{'name': 'Marka Adı'}]")]
        public string? BrandsJson { get; set; }

        [SwaggerSchema(Description = "Ürün kategorileri JSON formatında: [{'name': 'Kategori Adı'}]")]
        public string? ProductCategoriesJson { get; set; }

        [SwaggerSchema(Description = "Sergilenen ürünler JSON formatında: [{'name': 'Ürün Adı'}]")]
        public string? ExhibitedProductsJson { get; set; }

        [SwaggerSchema(Description = "Temsilci firmalar JSON formatında: [{'name': 'Firma Adı', 'country': 'Ülke', 'address': 'Adres', 'phone': 'Telefon', 'email': 'Email'}]")]
        public string? RepresentativeCompaniesJson { get; set; }

        // Parse edilmiş koleksiyonlar (service'te kullanılacak)
        [SwaggerIgnore]
        public ICollection<BranchDto>? Branches { get; set; }
        [SwaggerIgnore]
        public ICollection<BrandDto>? Brands { get; set; }
        [SwaggerIgnore]
        public ICollection<ProductCategoryDto>? ProductCategories { get; set; }
        [SwaggerIgnore]
        public ICollection<ExhibitedProductDto>? ExhibitedProducts { get; set; }
        [SwaggerIgnore]
        public ICollection<RepresentativeCompanyDto>? RepresentativeCompanies { get; set; }
    }
}
