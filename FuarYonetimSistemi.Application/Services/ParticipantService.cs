using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Domain.Entities;
using FuarYonetimSistemi.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Services
{
    public class ParticipantService : IParticipantService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ParticipantService(AppDbContext context, IFileService fileService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _fileService = fileService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ParticipantDto> CreateAsync(CreateParticipantDto dto)
        {
            var participant = new Participant
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                AuthFullName = dto.AuthFullName,
                CompanyName = dto.CompanyName,
                Address = dto.Address,
                Website = dto.Website,
                CreateDate = DateTime.UtcNow,
                IsDeleted = false,
                Branches = dto.Branches?.Select(b => new Branch { Name = b.Name }).ToList(),
                Brands = dto.Brands?.Select(b => new Brand { Name = b.Name }).ToList(),
                ProductCategories = dto.ProductCategories?.Select(c => new ProductCategory { Name = c.Name }).ToList(),
                ExhibitedProducts = dto.ExhibitedProducts?.Select(p => new ExhibitedProduct { Name = p.Name }).ToList(),
                RepresentativeCompanies = dto.RepresentativeCompanies?.Select(r => new RepresentativeCompany
                {
                    Name = r.Name,
                    Country = r.Country,
                    Address = r.Address,
                    District = r.District,
                    City = r.City,
                    Phone = r.Phone,
                    Email = r.Email,
                    Website = r.Website
                }).ToList()
            };

            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();

            return MapToDto(participant);
        }


        private async Task HandleLogoUpload(IFormFile file, Participant participant)
        {
            using var image = await Image.LoadAsync(file.OpenReadStream());
            if (file.Length > 1024 * 1024 || image.Width > 500 || image.Height > 300)
                throw new Exception("Logo 1MB'ı aşmamalı ve en fazla 500x300 piksel olmalı.");

            var (fileName, filePath, fileSize) = await _fileService.UploadFileAsync(file, "participant-logos");
            participant.LogoFileName = fileName;
            participant.LogoFilePath = filePath;
            participant.LogoContentType = file.ContentType;
            participant.LogoFileSize = fileSize;
            participant.LogoUploadDate = DateTime.UtcNow;
        }

        private ParticipantDto MapToDto(Participant p) => new()
        {
            Id = p.Id,
            FullName = p.FullName,
            Email = p.Email,
            Phone = p.Phone,
            AuthFullName = p.AuthFullName,
            CompanyName = p.CompanyName,
            Address = p.Address,
            Website = p.Website,
            CreateDate = p.CreateDate,
            Branches = p.Branches?.Select(b => new BranchDto { Name = b.Name }).ToList(),
            Brands = p.Brands?.Select(b => new BrandDto { Name = b.Name }).ToList(),
            ProductCategories = p.ProductCategories?.Select(c => new ProductCategoryDto { Name = c.Name }).ToList(),
            ExhibitedProducts = p.ExhibitedProducts?.Select(p => new ExhibitedProductDto { Name = p.Name }).ToList(),
            RepresentativeCompanies = p.RepresentativeCompanies?.Select(r => new RepresentativeCompanyDto
            {
                Name = r.Name,
                Country = r.Country,
                Address = r.Address,
                District = r.District,
                City = r.City,
                Phone = r.Phone,
                Email = r.Email,
                Website = r.Website
            }).ToList(),
            LogoFileName = p.LogoFileName,
            LogoFilePath = p.LogoFilePath,
            LogoContentType = p.LogoContentType,
            LogoFileSize = p.LogoFileSize,
            LogoUploadDate = p.LogoUploadDate
        };



        public async Task<ParticipantDto> UpdateAsync(Guid id, UpdateParticipantDto dto)
        {
            var participant = await _context.Participants
                .Include(p => p.Branches)
                .Include(p => p.Brands)
                .Include(p => p.ProductCategories)
                .Include(p => p.ExhibitedProducts)
                .Include(p => p.RepresentativeCompanies)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (participant == null || participant.IsDeleted) return null;

            participant.FullName = dto.FullName;
            participant.Email = dto.Email;
            participant.Phone = dto.Phone;
            participant.AuthFullName = dto.AuthFullName;
            participant.CompanyName = dto.CompanyName;
            participant.Address = dto.Address;
            participant.Website = dto.Website;

            UpdateCollection(participant.Branches, dto.Branches, (e, d) => e.Name == d.Name, d => new Branch { Name = d.Name }, (e, d) => e.Name = d.Name);
            UpdateCollection(participant.Brands, dto.Brands, (e, d) => e.Name == d.Name, d => new Brand { Name = d.Name }, (e, d) => e.Name = d.Name);
            UpdateCollection(participant.ProductCategories, dto.ProductCategories, (e, d) => e.Name == d.Name, d => new ProductCategory { Name = d.Name }, (e, d) => e.Name = d.Name);
            UpdateCollection(participant.ExhibitedProducts, dto.ExhibitedProducts, (e, d) => e.Name == d.Name, d => new ExhibitedProduct { Name = d.Name }, (e, d) => e.Name = d.Name);
            UpdateCollection(participant.RepresentativeCompanies, dto.RepresentativeCompanies, (e, d) => e.Name == d.Name && e.Country == d.Country, d => new RepresentativeCompany
            {
                Name = d.Name,
                Country = d.Country,
                Address = d.Address,
                District = d.District,
                City = d.City,
                Phone = d.Phone,
                Email = d.Email,
                Website = d.Website
            }, (e, d) =>
            {
                e.Name = d.Name;
                e.Country = d.Country;
                e.Address = d.Address;
                e.District = d.District;
                e.City = d.City;
                e.Phone = d.Phone;
                e.Email = d.Email;
                e.Website = d.Website;
            });

            if (dto.RemoveLogo && !string.IsNullOrEmpty(participant.LogoFilePath))
            {
                await _fileService.DeleteFileAsync(participant.LogoFilePath);
                participant.LogoFileName = participant.LogoFilePath = participant.LogoContentType = string.Empty;
                participant.LogoFileSize = 0;
                participant.LogoUploadDate = null;
            }
            else if (dto.LogoFile != null)
            {
                await HandleLogoUpload(dto.LogoFile, participant, true);
            }

            await _context.SaveChangesAsync();
            return MapToDto(participant);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var participant = await _context.Participants.FindAsync(id);
            if (participant == null || participant.IsDeleted) return false;

            if (!string.IsNullOrEmpty(participant.LogoFilePath))
            {
                await _fileService.DeleteFileAsync(participant.LogoFilePath);
            }

            participant.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ParticipantDto> GetByIdAsync(Guid id)
        {
            var participant = await _context.Participants
                .Include(p => p.Branches)
                .Include(p => p.Brands)
                .Include(p => p.ProductCategories)
                .Include(p => p.ExhibitedProducts)
                .Include(p => p.RepresentativeCompanies)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            return participant == null ? null : MapToDto(participant);
        }

        public async Task<IEnumerable<ParticipantDto>> GetAllAsync()
        {
            var participants = await _context.Participants
                .Where(p => !p.IsDeleted)
                .Include(p => p.Branches)
                .Include(p => p.Brands)
                .Include(p => p.ProductCategories)
                .Include(p => p.ExhibitedProducts)
                .Include(p => p.RepresentativeCompanies)
                .ToListAsync();
            return participants.Select(MapToDto);
        }

        public async Task<PagedResult<ParticipantDto>> FilterPagedAsync(ParticipantFilterDto filter)
        {
            var query = _context.Participants
                .Where(p => !p.IsDeleted)
                .Include(p => p.Branches)
                .Include(p => p.Brands)
                .Include(p => p.ProductCategories)
                .Include(p => p.ExhibitedProducts)
                .Include(p => p.RepresentativeCompanies)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.FullName))
                query = query.Where(p => p.FullName.Contains(filter.FullName));
            if (!string.IsNullOrWhiteSpace(filter.Email))
                query = query.Where(p => p.Email.Contains(filter.Email));
            if (!string.IsNullOrWhiteSpace(filter.AuthFullName))
                query = query.Where(p => p.AuthFullName.Contains(filter.AuthFullName));
            if (!string.IsNullOrWhiteSpace(filter.CompanyName))
                query = query.Where(p => p.CompanyName.Contains(filter.CompanyName));
            if (filter.CreateDate.HasValue)
            {
                var date = filter.CreateDate.Value.Date;
                var next = date.AddDays(1);
                query = query.Where(p => p.CreateDate >= date && p.CreateDate < next);
            }
            if (filter.HasLogo.HasValue)
            {
                query = filter.HasLogo.Value ? query.Where(p => !string.IsNullOrEmpty(p.LogoFilePath)) : query.Where(p => string.IsNullOrEmpty(p.LogoFilePath));
            }

            query = filter.SortBy?.ToLower() switch
            {
                "fullname" => filter.IsDescending ? query.OrderByDescending(p => p.FullName) : query.OrderBy(p => p.FullName),
                "email" => filter.IsDescending ? query.OrderByDescending(p => p.Email) : query.OrderBy(p => p.Email),
                "authfullname" => filter.IsDescending ? query.OrderByDescending(p => p.AuthFullName) : query.OrderBy(p => p.AuthFullName),
                "companyname" => filter.IsDescending ? query.OrderByDescending(p => p.CompanyName) : query.OrderBy(p => p.CompanyName),
                _ => filter.IsDescending ? query.OrderByDescending(p => p.CreateDate) : query.OrderBy(p => p.CreateDate)
            };

            var totalCount = await query.CountAsync();
            var items = await query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();
            return new PagedResult<ParticipantDto> { Items = items.Select(MapToDto), TotalCount = totalCount };
        }

        public async Task<byte[]> GetLogoAsync(Guid id)
        {
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            return (participant == null || !_fileService.FileExists(participant.LogoFilePath)) ? null : await _fileService.GetFileContentAsync(participant.LogoFilePath);
        }

        private async Task HandleLogoUpload(IFormFile logoFile, Participant participant, bool deleteOld = false)
        {
            if (logoFile.Length > 1024 * 1024)
                throw new Exception("Logo dosya boyutu 1 MB'dan büyük olamaz.");

            using var imageStream = logoFile.OpenReadStream();
            using var image = await Image.LoadAsync(imageStream);
            if (image.Width > 500 || image.Height > 300)
                throw new Exception("Logo maksimum 500x300 piksel olmalıdır.");

            if (deleteOld && !string.IsNullOrEmpty(participant.LogoFilePath))
                await _fileService.DeleteFileAsync(participant.LogoFilePath);

            var (fileName, filePath, fileSize) = await _fileService.UploadFileAsync(logoFile, "participant-logos");
            participant.LogoFileName = fileName;
            participant.LogoFilePath = filePath;
            participant.LogoContentType = logoFile.ContentType;
            participant.LogoFileSize = fileSize;
            participant.LogoUploadDate = DateTime.UtcNow;
        }

     

        private void UpdateCollection<TEntity, TDto>(
            ICollection<TEntity> existing,
            ICollection<TDto> updated,
            Func<TEntity, TDto, bool> comparer,
            Func<TDto, TEntity> create,
            Action<TEntity, TDto> update) where TEntity : class
        {
            updated ??= new List<TDto>();
            var toRemove = existing.Where(e => !updated.Any(d => comparer(e, d))).ToList();
            foreach (var item in toRemove) existing.Remove(item);

            foreach (var dto in updated)
            {
                var entity = existing.FirstOrDefault(e => comparer(e, dto));
                if (entity == null) existing.Add(create(dto));
                else update(entity, dto);
            }
        }
    }
}
