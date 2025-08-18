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
            // ✅ Duplicate email kontrolü
            var existingParticipant = await _context.Participants
                .FirstOrDefaultAsync(p => p.Email == dto.Email && !p.IsDeleted);

            if (existingParticipant != null)
            {
                throw new InvalidOperationException($"Bu email adresi ({dto.Email}) ile kayıtlı bir katılımcı zaten mevcut.");
            }

            // ✅ Transaction kullan
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
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
                    Branches = dto.Branches?.Select(b => new Branch { Name = b.Name }).ToList() ?? new List<Branch>(),
                    Brands = dto.Brands?.Select(b => new Brand { Name = b.Name }).ToList() ?? new List<Brand>(),
                    ProductCategories = dto.ProductCategories?.Select(c => new ProductCategory { Name = c.Name }).ToList() ?? new List<ProductCategory>(),
                    ExhibitedProducts = dto.ExhibitedProducts?.Select(p => new ExhibitedProduct { Name = p.Name }).ToList() ?? new List<ExhibitedProduct>(),
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
                    }).ToList() ?? new List<RepresentativeCompany>()
                };

                // ✅ Önce DB'ye kaydet
                _context.Participants.Add(participant);
                await _context.SaveChangesAsync();

                // ✅ DB kaydı başarılıysa logo yükle
                if (dto.LogoFile != null)
                {
                    try
                    {
                        await HandleLogoUpload(dto.LogoFile, participant);
                        await _context.SaveChangesAsync(); // Logo bilgilerini güncelle
                    }
                    catch (Exception ex)
                    {
                        // Logo yükleme başarısız, transaction rollback
                        await transaction.RollbackAsync();
                        throw new InvalidOperationException($"Logo yüklenirken hata oluştu: {ex.Message}", ex);
                    }
                }

                // ✅ Her şey başarılı, commit
                await transaction.CommitAsync();

                return MapToDto(participant);
            }
            catch (Exception)
            {
                // Hata durumunda rollback
                await transaction.RollbackAsync();
                throw;
            }
        }

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

            // ✅ Email değişikliği kontrolü (farklı katılımcıda aynı email var mı?)
            if (participant.Email != dto.Email)
            {
                var existingParticipant = await _context.Participants
                    .FirstOrDefaultAsync(p => p.Email == dto.Email && !p.IsDeleted && p.Id != id);

                if (existingParticipant != null)
                {
                    throw new InvalidOperationException($"Bu email adresi ({dto.Email}) ile kayıtlı başka bir katılımcı zaten mevcut.");
                }
            }

            participant.FullName = dto.FullName;
            participant.Email = dto.Email;
            participant.Phone = dto.Phone;
            participant.AuthFullName = dto.AuthFullName;
            participant.CompanyName = dto.CompanyName;
            participant.Address = dto.Address;
            participant.Website = dto.Website;

            // ✅ UpdateCollection çağrılarını düzelt - DTO tiplerini kullan
            UpdateCollection(participant.Branches, dto.Branches,
                (e, d) => e.Name == d.Name,
                d => new Branch { Name = d.Name },
                (e, d) => e.Name = d.Name);

            UpdateCollection(participant.Brands, dto.Brands,
                (e, d) => e.Name == d.Name,
                d => new Brand { Name = d.Name },
                (e, d) => e.Name = d.Name);

            UpdateCollection(participant.ProductCategories, dto.ProductCategories,
                (e, d) => e.Name == d.Name,
                d => new ProductCategory { Name = d.Name },
                (e, d) => e.Name = d.Name);

            UpdateCollection(participant.ExhibitedProducts, dto.ExhibitedProducts,
                (e, d) => e.Name == d.Name,
                d => new ExhibitedProduct { Name = d.Name },
                (e, d) => e.Name = d.Name);

            UpdateCollection(participant.RepresentativeCompanies, dto.RepresentativeCompanies,
                (e, d) => e.Name == d.Name && e.Country == d.Country,
                d => new RepresentativeCompany
                {
                    Name = d.Name,
                    Country = d.Country,
                    Address = d.Address,
                    District = d.District,
                    City = d.City,
                    Phone = d.Phone,
                    Email = d.Email,
                    Website = d.Website
                },
                (e, d) =>
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

            // Logo işlemleri
            if (dto.RemoveLogo && !string.IsNullOrEmpty(participant.LogoFilePath))
            {
                await _fileService.DeleteFileAsync(participant.LogoFilePath);
                participant.LogoFileName = string.Empty;
                participant.LogoFilePath = string.Empty;
                participant.LogoContentType = string.Empty;
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
                try
                {
                    await _fileService.DeleteFileAsync(participant.LogoFilePath);
                }
                catch (Exception)
                {
                    // Log error but continue with soft delete
                    // File might already be deleted or service unavailable
                }
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
                query = filter.HasLogo.Value
                    ? query.Where(p => !string.IsNullOrEmpty(p.LogoFilePath))
                    : query.Where(p => string.IsNullOrEmpty(p.LogoFilePath));
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

            return new PagedResult<ParticipantDto>
            {
                Items = items.Select(MapToDto),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<byte[]> GetLogoAsync(Guid id)
        {
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            return (participant == null || string.IsNullOrEmpty(participant.LogoFilePath) || !_fileService.FileExists(participant.LogoFilePath))
                ? null
                : await _fileService.GetFileContentAsync(participant.LogoFilePath);
        }

        // ✅ TEK HandleLogoUpload metodu - duplicate kaldırıldı
        private async Task HandleLogoUpload(IFormFile logoFile, Participant participant, bool deleteOld = false)
        {
            // Double validation (controller'da da yapılıyor ama güvenlik için)
            if (logoFile.Length > 1024 * 1024)
                throw new InvalidOperationException("Logo dosya boyutu 1 MB'dan büyük olamaz.");

            try
            {
                using var imageStream = logoFile.OpenReadStream();
                using var image = await Image.LoadAsync(imageStream);

                if (image.Width > 500 || image.Height > 300)
                    throw new InvalidOperationException("Logo maksimum 500x300 piksel olmalıdır.");
            }
            catch (Exception ex) when (!(ex is InvalidOperationException))
            {
                throw new InvalidOperationException("Geçersiz resim dosyası.", ex);
            }

            // Eski logo varsa sil
            if (deleteOld && !string.IsNullOrEmpty(participant.LogoFilePath))
            {
                try
                {
                    await _fileService.DeleteFileAsync(participant.LogoFilePath);
                }
                catch (Exception)
                {
                    // Log error but don't fail the upload
                    // _logger.LogWarning(ex, "Eski logo dosyası silinemedi: {FilePath}", participant.LogoFilePath);
                }
            }

            try
            {
                var (fileName, filePath, fileSize) = await _fileService.UploadFileAsync(logoFile, "participant-logos");

                participant.LogoFileName = fileName;
                participant.LogoFilePath = filePath;
                participant.LogoContentType = logoFile.ContentType;
                participant.LogoFileSize = fileSize;
                participant.LogoUploadDate = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Logo dosyası kaydedilemedi.", ex);
            }
        }

        // ✅ MapToDto metodunu düzelt - Id mapping ekle
        private ParticipantDto MapToDto(Participant p)
        {
            var baseUrl = _httpContextAccessor.HttpContext?.Request != null
                ? $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}"
                : "";

            return new ParticipantDto
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

                // ✅ Id'lerle birlikte mapping
                Branches = p.Branches?.Select(b => new BranchDto { Id = b.Id, Name = b.Name }).ToList(),
                Brands = p.Brands?.Select(b => new BrandDto { Id = b.Id, Name = b.Name }).ToList(),
                ProductCategories = p.ProductCategories?.Select(c => new ProductCategoryDto { Id = c.Id, Name = c.Name }).ToList(),
                ExhibitedProducts = p.ExhibitedProducts?.Select(e => new ExhibitedProductDto { Id = e.Id, Name = e.Name }).ToList(),
                RepresentativeCompanies = p.RepresentativeCompanies?.Select(r => new RepresentativeCompanyDto
                {
                    Id = r.Id,
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
                LogoUploadDate = p.LogoUploadDate,

                // ✅ Computed properties
                LogoUrl = !string.IsNullOrEmpty(p.LogoFilePath)
                    ? $"{baseUrl}/api/participants/{p.Id}/logo"
                    : null
            };
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
            foreach (var item in toRemove)
                existing.Remove(item);

            foreach (var dto in updated)
            {
                var entity = existing.FirstOrDefault(e => comparer(e, dto));
                if (entity == null)
                    existing.Add(create(dto));
                else
                    update(entity, dto);
            }
        }
    }
}