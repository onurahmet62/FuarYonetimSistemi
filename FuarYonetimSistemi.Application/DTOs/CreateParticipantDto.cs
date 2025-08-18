using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class CreateParticipantDto
    {
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

        [SwaggerSchema(Description = "Logo dosyası (max 1MB, 500x300px)")]
        public IFormFile? LogoFile { get; set; }

        // JSON string alanları
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

        // Bu alanlar Swagger'da görünmeyecek
        [SwaggerIgnore]
        public List<CreateBranchDto>? Branches { get; set; }
        [SwaggerIgnore]
        public List<CreateBrandDto>? Brands { get; set; }
        [SwaggerIgnore]
        public List<CreateProductCategoryDto>? ProductCategories { get; set; }
        [SwaggerIgnore]
        public List<CreateExhibitedProductDto>? ExhibitedProducts { get; set; }
        [SwaggerIgnore]
        public List<CreateRepresentativeCompanyDto>? RepresentativeCompanies { get; set; }
    }

    // ✅ Child DTO'lara da validation ekle
    public class CreateBranchDto
    {
        [Required(ErrorMessage = "Şube adı zorunludur")]
        [StringLength(200, ErrorMessage = "Şube adı en fazla 200 karakter olabilir")]
        public string Name { get; set; } = string.Empty;
    }

    public class CreateBrandDto
    {
        [Required(ErrorMessage = "Marka adı zorunludur")]
        [StringLength(200, ErrorMessage = "Marka adı en fazla 200 karakter olabilir")]
        public string Name { get; set; } = string.Empty;
    }

    public class CreateProductCategoryDto
    {
        [Required(ErrorMessage = "Ürün kategorisi zorunludur")]
        [StringLength(150, ErrorMessage = "Ürün kategorisi en fazla 150 karakter olabilir")]
        public string Name { get; set; } = string.Empty;
    }

    public class CreateExhibitedProductDto
    {
        [Required(ErrorMessage = "Ürün adı zorunludur")]
        [StringLength(200, ErrorMessage = "Ürün adı en fazla 200 karakter olabilir")]
        public string Name { get; set; } = string.Empty;
    }

    public class CreateRepresentativeCompanyDto
    {
        [Required(ErrorMessage = "Temsilci firma adı zorunludur")]
        [StringLength(200, ErrorMessage = "Firma adı en fazla 200 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ülke bilgisi zorunludur")]
        [StringLength(100, ErrorMessage = "Ülke en fazla 100 karakter olabilir")]
        public string Country { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Adres en fazla 300 karakter olabilir")]
        public string? Address { get; set; }

        [StringLength(100, ErrorMessage = "İlçe en fazla 100 karakter olabilir")]
        public string? District { get; set; }

        [StringLength(100, ErrorMessage = "Şehir en fazla 100 karakter olabilir")]
        public string? City { get; set; }

        [StringLength(50, ErrorMessage = "Telefon en fazla 50 karakter olabilir")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        public string? Phone { get; set; }

        [StringLength(150, ErrorMessage = "Email en fazla 150 karakter olabilir")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string? Email { get; set; }

        [StringLength(200, ErrorMessage = "Website en fazla 200 karakter olabilir")]
        public string? Website { get; set; }
    }






}
