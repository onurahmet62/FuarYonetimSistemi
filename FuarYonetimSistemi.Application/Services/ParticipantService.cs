using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Domain.Entities;
using FuarYonetimSistemi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

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
                CreateDate = DateTime.UtcNow,
                AuthFullName = dto.AuthFullName,
                CompanyName = dto.CompanyName,
                Address = dto.Address,
                Website = dto.Website,
                Branches = dto.Branches,
                IsDeleted = false
            };

            if (dto.LogoFile != null)
            {
                try
                {
                    // Dosya boyutu sınırı (1 MB)
                    if (dto.LogoFile.Length > 1024 * 1024)
                        throw new Exception("Logo dosya boyutu 1 MB'dan büyük olamaz.");

                    // Piksel boyutu kontrolü (ImageSharp)
                    using var imageStream = dto.LogoFile.OpenReadStream();
                    using var image = await Image.LoadAsync(imageStream);

                    if (image.Width > 500 || image.Height > 300)
                        throw new Exception("Logo maksimum 500x300 piksel boyutunda olmalıdır.");

                    var (fileName, filePath, fileSize) = await _fileService.UploadFileAsync(dto.LogoFile, "participant-logos");

                    participant.LogoFileName = fileName;
                    participant.LogoFilePath = filePath;
                    participant.LogoContentType = dto.LogoFile.ContentType;
                    participant.LogoFileSize = fileSize;
                    participant.LogoUploadDate = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Logo yüklenemedi: {ex.Message}");
                }
            }

            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();

            return MapToDto(participant);
        }

        public async Task<ParticipantDto> UpdateAsync(Guid id, UpdateParticipantDto dto)
        {
            var participant = await _context.Participants.FindAsync(id);
            if (participant == null || participant.IsDeleted)
                return null;

            participant.FullName = dto.FullName;
            participant.Email = dto.Email;
            participant.Phone = dto.Phone;
            participant.AuthFullName = dto.AuthFullName;
            participant.CompanyName = dto.CompanyName;
            participant.Address = dto.Address;
            participant.Website = dto.Website;
            participant.Branches = dto.Branches;

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
                try
                {
                    // Dosya boyutu sınırı (1 MB)
                    if (dto.LogoFile.Length > 1024 * 1024)
                        throw new Exception("Logo dosya boyutu 1 MB'dan büyük olamaz.");

                    // Piksel boyutu kontrolü (ImageSharp)
                    using var imageStream = dto.LogoFile.OpenReadStream();
                    using var image = await Image.LoadAsync(imageStream);

                    if (image.Width > 500 || image.Height > 300)
                        throw new Exception("Logo maksimum 500x300 piksel boyutunda olmalıdır.");

                    if (!string.IsNullOrEmpty(participant.LogoFilePath))
                    {
                        await _fileService.DeleteFileAsync(participant.LogoFilePath);
                    }

                    var (fileName, filePath, fileSize) = await _fileService.UploadFileAsync(dto.LogoFile, "participant-logos");

                    participant.LogoFileName = fileName;
                    participant.LogoFilePath = filePath;
                    participant.LogoContentType = dto.LogoFile.ContentType;
                    participant.LogoFileSize = fileSize;
                    participant.LogoUploadDate = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Logo güncellenemedi: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();
            return MapToDto(participant);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var participant = await _context.Participants.FindAsync(id);
            if (participant == null || participant.IsDeleted)
                return false;

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
                .Where(p => p.Id == id && !p.IsDeleted)
                .FirstOrDefaultAsync();

            if (participant == null)
                return null;

            return MapToDto(participant);
        }

        public async Task<IEnumerable<ParticipantDto>> GetAllAsync()
        {
            var participants = await _context.Participants
                .Where(p => !p.IsDeleted)
                .ToListAsync();

            return participants.Select(MapToDto);
        }

        public async Task<PagedResult<ParticipantDto>> FilterPagedAsync(ParticipantFilterDto filter)
        {
            var query = _context.Participants
                .Where(p => !p.IsDeleted)
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
                query = query.Where(p => p.CreateDate.Date == filter.CreateDate.Value.Date);

            if (filter.HasLogo.HasValue)
            {
                if (filter.HasLogo.Value)
                    query = query.Where(p => !string.IsNullOrEmpty(p.LogoFilePath));
                else
                    query = query.Where(p => string.IsNullOrEmpty(p.LogoFilePath));
            }

            query = filter.SortBy?.ToLower() switch
            {
                "fullname" => filter.IsDescending ? query.OrderByDescending(p => p.FullName) : query.OrderBy(p => p.FullName),
                "email" => filter.IsDescending ? query.OrderByDescending(p => p.Email) : query.OrderBy(p => p.Email),
                "authfullname" => filter.IsDescending ? query.OrderByDescending(p => p.AuthFullName) : query.OrderBy(p => p.AuthFullName),
                "companyname" => filter.IsDescending ? query.OrderByDescending(p => p.CompanyName) : query.OrderBy(p => p.CompanyName),
                "createdate" => filter.IsDescending ? query.OrderByDescending(p => p.CreateDate) : query.OrderBy(p => p.CreateDate),
                _ => filter.IsDescending ? query.OrderByDescending(p => p.CreateDate) : query.OrderBy(p => p.CreateDate)
            };

            var totalCount = await query.CountAsync();

            var participants = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var items = participants.Select(MapToDto);

            return new PagedResult<ParticipantDto>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<byte[]> GetLogoAsync(Guid participantId)
        {
            var participant = await _context.Participants
                .Where(p => p.Id == participantId && !p.IsDeleted && !string.IsNullOrEmpty(p.LogoFilePath))
                .FirstOrDefaultAsync();

            if (participant == null)
                return null;

            if (!_fileService.FileExists(participant.LogoFilePath))
                return null;

            return await _fileService.GetFileContentAsync(participant.LogoFilePath);
        }

        private ParticipantDto MapToDto(Participant participant)
        {
            var dto = new ParticipantDto
            {
                Id = participant.Id,
                FullName = participant.FullName,
                Email = participant.Email,
                Phone = participant.Phone,
                CreateDate = participant.CreateDate,
                AuthFullName = participant.AuthFullName,
                CompanyName = participant.CompanyName,
                Address = participant.Address,
                Website = participant.Website,
                Branches = participant.Branches,
                LogoFileName = participant.LogoFileName,
                LogoFilePath = participant.LogoFilePath,
                LogoContentType = participant.LogoContentType,
                LogoFileSize = participant.LogoFileSize,
                LogoUploadDate = participant.LogoUploadDate
            };

            if (!string.IsNullOrEmpty(participant.LogoFilePath))
            {
                var request = _httpContextAccessor.HttpContext?.Request;
                if (request != null)
                {
                    dto.LogoUrl = $"{request.Scheme}://{request.Host}/{participant.LogoFilePath}";
                }
            }

            return dto;
        }
    }
}
