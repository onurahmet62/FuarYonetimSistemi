using ClosedXML.Excel;
using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager,SalesPerson")]
    public class ParticipantsController : ControllerBase
    {
        private readonly IParticipantService _participantService;
        private readonly IPdfService _pdfService;

        public ParticipantsController(IParticipantService participantService, IPdfService pdfService)
        {
            _participantService = participantService;
            _pdfService = pdfService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _participantService.GetAllAsync());

        // ✅ Düzeltilmiş Controller POST Metodu
        [HttpPost]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(
            Summary = "Yeni katılımcı oluştur",
            Description = "Fuar sistemine yeni bir katılımcı ekler. Logo dosyası, şube, marka, ürün kategorisi ve temsilci firma bilgileri ile birlikte kayıt yapılabilir.",
            OperationId = "CreateParticipant"
        )]
        [SwaggerResponse(201, "Katılımcı başarıyla oluşturuldu", typeof(ParticipantDto))]
        [SwaggerResponse(400, "Geçersiz veri")]
        [SwaggerResponse(500, "Sunucu hatası")]
        public async Task<IActionResult> Create([FromForm] CreateParticipantDto dto)
        {
            try
            {
                // ✅ Model validation kontrolü
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // ✅ Logo validation (service'ten önce)
                if (dto.LogoFile != null)
                {
                    var logoValidation = ValidateLogoFile(dto.LogoFile);
                    if (!logoValidation.IsValid)
                    {
                        return BadRequest(new { message = logoValidation.ErrorMessage });
                    }
                }

                // ✅ JSON parsing ile daha iyi error handling
                try
                {
                    dto.Branches = ParseJsonList<CreateBranchDto>(dto.BranchesJson);
                    dto.Brands = ParseJsonList<CreateBrandDto>(dto.BrandsJson);
                    dto.ProductCategories = ParseJsonList<CreateProductCategoryDto>(dto.ProductCategoriesJson);
                    dto.ExhibitedProducts = ParseJsonList<CreateExhibitedProductDto>(dto.ExhibitedProductsJson);
                    dto.RepresentativeCompanies = ParseJsonList<CreateRepresentativeCompanyDto>(dto.RepresentativeCompaniesJson);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { message = "JSON formatı geçersiz", detail = ex.Message });
                }

                var participant = await _participantService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = participant.Id }, participant);
            }
            catch (InvalidOperationException ex)
            {
                // Business logic hataları (duplicate email vb.)
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the error (ILogger kullanılmalı)
                return StatusCode(500, new
                {
                    message = "Katılımcı oluşturulurken bir hata oluştu.",
                    detail = ex.Message
                });
            }
        }

        // ✅ Logo validation helper metodu
        private (bool IsValid, string ErrorMessage) ValidateLogoFile(IFormFile logoFile)
        {
            const int MaxFileSizeBytes = 1024 * 1024; // 1MB
            const int MaxWidth = 500;
            const int MaxHeight = 300;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };

            // Dosya boyutu kontrolü
            if (logoFile.Length > MaxFileSizeBytes)
            {
                return (false, "Logo dosya boyutu 1 MB'dan büyük olamaz.");
            }

            // Dosya uzantısı kontrolü
            var fileExtension = Path.GetExtension(logoFile.FileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
            {
                return (false, "Desteklenmeyen dosya formatı. Sadece JPG, PNG ve GIF dosyaları kabul edilir.");
            }

            // MIME type kontrolü
            if (!allowedMimeTypes.Contains(logoFile.ContentType?.ToLowerInvariant()))
            {
                return (false, "Geçersiz dosya tipi. Sadece resim dosyaları kabul edilir.");
            }

            // Dosya adı kontrolü
            if (string.IsNullOrWhiteSpace(logoFile.FileName))
            {
                return (false, "Geçerli bir dosya adı gereklidir.");
            }

            return (true, string.Empty);
        }

        // ✅ Geliştirilmiş JSON parsing
        private static List<T>? ParseJsonList<T>(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                // Trim ve basic validation
                json = json.Trim();

                if (json.StartsWith("{") && json.EndsWith("}"))
                {
                    // Tek obje
                    var single = JsonConvert.DeserializeObject<T>(json);
                    return single != null ? new List<T> { single } : null;
                }
                else if (json.StartsWith("[") && json.EndsWith("]"))
                {
                    // Array
                    return JsonConvert.DeserializeObject<List<T>>(json);
                }
                else
                {
                    throw new ArgumentException($"JSON formatı geçersiz. Obje {{}} veya array [] bekleniyor: {json}");
                }
            }
            catch (JsonException ex)
            {
                throw new ArgumentException($"JSON parse hatası: {ex.Message}. Alınan veri: {json}", ex);
            }
        }






      




        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id) =>
            await _participantService.GetByIdAsync(id) is { } p ? Ok(p) : NotFound();

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(
            Summary = "Katılımcı bilgilerini güncelle",
            Description = "Mevcut katılımcının bilgilerini günceller. Logo dosyası ve diğer tüm bilgiler değiştirilebilir.",
            OperationId = "UpdateParticipant"
        )]
        [SwaggerResponse(200, "Katılımcı başarıyla güncellendi", typeof(ParticipantDto))]
        [SwaggerResponse(400, "Geçersiz veri")]
        [SwaggerResponse(404, "Katılımcı bulunamadı")]
        [SwaggerResponse(500, "Sunucu hatası")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateParticipantDto dto)
        {
            try
            {
                var participant = await _participantService.UpdateAsync(id, dto);
                return participant == null ? NotFound() : Ok(participant);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Katılımcı güncellenirken bir hata oluştu." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _participantService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPaged([FromBody] ParticipantFilterDto filter)
            => Ok(await _participantService.FilterPagedAsync(filter));

        [HttpGet("{id}/logo")]
        public async Task<IActionResult> GetLogo(Guid id)
        {
            var logoBytes = await _participantService.GetLogoAsync(id);
            if (logoBytes == null)
                return NotFound(new { message = "Logo bulunamadı." });

            var participant = await _participantService.GetByIdAsync(id);
            return participant == null
                ? NotFound()
                : File(logoBytes, participant.LogoContentType ?? "image/jpeg", participant.LogoFileName);
        }

        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportParticipantsToExcel()
        {
            var participants = await _participantService.GetAllAsync();
            if (participants == null || !participants.Any())
                return BadRequest(new { message = "Dışa aktarılacak katılımcı bulunamadı." });

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Participants");

            var headers = new[] {
                "Ad Soyad", "Firma", "Telefon", "Email", "Yetkili Kişi", "Adres", "Web Sitesi",
                "Şubeler", "Markalar", "Ürün Kategorileri", "Sergilenen Ürünler", "Temsilci Firmalar", "Logo", "Kayıt Tarihi"
            };

            for (int i = 0; i < headers.Length; i++)
                worksheet.Cell(1, i + 1).Value = headers[i];

            worksheet.Row(1).Style.Font.Bold = true;

            int row = 2;
            foreach (var p in participants)
            {
                worksheet.Cell(row, 1).Value = p.FullName;
                worksheet.Cell(row, 2).Value = p.CompanyName;
                worksheet.Cell(row, 3).Value = p.Phone;
                worksheet.Cell(row, 4).Value = p.Email;
                worksheet.Cell(row, 5).Value = p.AuthFullName;
                worksheet.Cell(row, 6).Value = p.Address;
                worksheet.Cell(row, 7).Value = p.Website;
                worksheet.Cell(row, 8).Value = string.Join(", ", p.Branches?.Select(b => b.Name) ?? []);
                worksheet.Cell(row, 9).Value = string.Join(", ", p.Brands?.Select(b => b.Name) ?? []);
                worksheet.Cell(row, 10).Value = string.Join(", ", p.ProductCategories?.Select(c => c.Name) ?? []);
                worksheet.Cell(row, 11).Value = string.Join(", ", p.ExhibitedProducts?.Select(e => e.Name) ?? []);
                worksheet.Cell(row, 12).Value = string.Join("; ", p.RepresentativeCompanies?.Select(rc => $"{rc.Name} ({rc.Country})") ?? []);
                worksheet.Cell(row, 13).Value = string.IsNullOrWhiteSpace(p.LogoFileName) ? "Yok" : "Var";
                worksheet.Cell(row, 14).Value = p.CreateDate.ToString("dd.MM.yyyy HH:mm");
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Participants_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }

        [HttpGet("{id}/export-pdf")]
        public async Task<IActionResult> ExportParticipantToPdf(Guid id)
        {
            var participant = await _participantService.GetByIdAsync(id);
            if (participant == null) return NotFound();

            try
            {
                var pdfBytes = await _pdfService.GenerateParticipantPdfAsync(participant);
                var fileName = $"{participant.FullName.Replace(" ", "_")}_Bilgileri_{DateTime.Now:yyyyMMdd}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch
            {
                return StatusCode(500, new { message = "PDF oluşturulurken bir hata oluştu." });
            }
        }

        [HttpGet("export-pdf")]
        public async Task<IActionResult> ExportAllParticipantsToPdf()
        {
            try
            {
                var participants = await _participantService.GetAllAsync();
                if (participants == null || !participants.Any())
                    return BadRequest(new { message = "Katılımcı verisi bulunamadı." });

                var pdfBytes = await _pdfService.GenerateParticipantListPdfAsync(participants);
                var fileName = $"Katilimci_Listesi_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch
            {
                return StatusCode(500, new { message = "PDF oluşturulurken bir hata oluştu." });
            }
        }

        [HttpPost("export-pdf-filtered")]
        public async Task<IActionResult> ExportFilteredParticipantsToPdf([FromBody] ParticipantFilterDto filter)
        {
            try
            {
                filter.PageNumber = 1;
                filter.PageSize = int.MaxValue;

                var result = await _participantService.FilterPagedAsync(filter);
                if (result.Items == null || !result.Items.Any())
                    return BadRequest(new { message = "Filtreye uygun katılımcı bulunamadı." });

                var pdfBytes = await _pdfService.GenerateParticipantListPdfAsync(result.Items);
                var fileName = $"Katilimci_Listesi_Filtreli_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch
            {
                return StatusCode(500, new { message = "PDF oluşturulurken bir hata oluştu." });
            }
        }
    }
}
