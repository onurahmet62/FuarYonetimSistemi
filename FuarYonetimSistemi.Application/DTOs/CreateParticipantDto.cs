using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class CreateParticipantDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string AuthFullName { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }

        public IFormFile? LogoFile { get; set; }

        // JSON string alınan alanlar
        public string? BranchesJson { get; set; }
        public string? BrandsJson { get; set; }
        public string? ProductCategoriesJson { get; set; }
        public string? ExhibitedProductsJson { get; set; }
        public string? RepresentativeCompaniesJson { get; set; }

        // Parse sonrası atanan listeler (serviste kullanılacak)
        public List<CreateBranchDto>? Branches { get; set; }
        public List<CreateBrandDto>? Brands { get; set; }
        public List<CreateProductCategoryDto>? ProductCategories { get; set; }
        public List<CreateExhibitedProductDto>? ExhibitedProducts { get; set; }
        public List<CreateRepresentativeCompanyDto>? RepresentativeCompanies { get; set; }
    }


    public class CreateBranchDto { public string Name { get; set; } }
    public class CreateBrandDto { public string Name { get; set; } }
    public class CreateProductCategoryDto { public string Name { get; set; } }
    public class CreateExhibitedProductDto { public string Name { get; set; } }
    public class CreateRepresentativeCompanyDto
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string? Address { get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
    }






}
