using ClosedXML.Excel;
using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
        {
            var participants = await _participantService.GetAllAsync();
            return Ok(participants);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var participant = await _participantService.GetByIdAsync(id);
            if (participant == null)
                return NotFound();

            return Ok(participant);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateParticipantDto dto)
        {
            try
            {
                var participant = await _participantService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = participant.Id }, participant);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Katılımcı oluşturulurken bir hata oluştu." });
            }
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateParticipantDto dto)
        {
            try
            {
                var participant = await _participantService.UpdateAsync(id, dto);
                if (participant == null)
                    return NotFound();

                return Ok(participant);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Katılımcı güncellenirken bir hata oluştu." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _participantService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPaged([FromBody] ParticipantFilterDto filter)
        {
            var result = await _participantService.FilterPagedAsync(filter);
            return Ok(result);
        }

        // Logo İşlemleri
        [HttpGet("{id}/logo")]
        public async Task<IActionResult> GetLogo(Guid id)
        {
            var logoBytes = await _participantService.GetLogoAsync(id);
            if (logoBytes == null)
                return NotFound(new { message = "Logo bulunamadı." });

            var participant = await _participantService.GetByIdAsync(id);
            if (participant == null)
                return NotFound();

            return File(logoBytes, participant.LogoContentType ?? "image/jpeg", participant.LogoFileName);
        }

        // Excel Export
        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportParticipantsToExcel()
        {
            var participants = await _participantService.GetAllAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Participants");

            // Başlıklar
            worksheet.Cell(1, 1).Value = "Ad Soyad";
            worksheet.Cell(1, 2).Value = "Firma";
            worksheet.Cell(1, 3).Value = "Telefon";
            worksheet.Cell(1, 4).Value = "Email";
            worksheet.Cell(1, 5).Value = "Yetkili Kişi";
            worksheet.Cell(1, 6).Value = "Adres";
            worksheet.Cell(1, 7).Value = "Web Sitesi";
            worksheet.Cell(1, 8).Value = "Şubeler";
            worksheet.Cell(1, 9).Value = "Logo";
            worksheet.Cell(1, 10).Value = "Kayıt Tarihi";

            // Başlık satırını kalın yap
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
                worksheet.Cell(row, 8).Value = p.Branches;
                worksheet.Cell(row, 9).Value = !string.IsNullOrEmpty(p.LogoFilePath) ? "Var" : "Yok";
                worksheet.Cell(row, 10).Value = p.CreateDate.ToString("dd.MM.yyyy HH:mm");
                row++;
            }

            // Sütun genişliklerini otomatik ayarla
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Participants_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }

        // PDF Export - Tekil Katılımcı
        [HttpGet("{id}/export-pdf")]
        public async Task<IActionResult> ExportParticipantToPdf(Guid id)
        {
            var participant = await _participantService.GetByIdAsync(id);
            if (participant == null)
                return NotFound();

            try
            {
                var pdfBytes = await _pdfService.GenerateParticipantPdfAsync(participant);
                var fileName = $"{participant.FullName.Replace(" ", "_")}_Bilgileri_{DateTime.Now:yyyyMMdd}.pdf";

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "PDF oluşturulurken bir hata oluştu." });
            }
        }

        // PDF Export - Tüm Katılımcılar
        [HttpGet("export-pdf")]
        public async Task<IActionResult> ExportAllParticipantsToPdf()
        {
            try
            {
                var participants = await _participantService.GetAllAsync();
                var pdfBytes = await _pdfService.GenerateParticipantListPdfAsync(participants);
                var fileName = $"Katilimci_Listesi_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "PDF oluşturulurken bir hata oluştu." });
            }
        }

        // PDF Export - Filtrelenmiş Katılımcılar
        [HttpPost("export-pdf-filtered")]
        public async Task<IActionResult> ExportFilteredParticipantsToPdf([FromBody] ParticipantFilterDto filter)
        {
            try
            {
                // Tüm kayıtları al (sayfalama olmadan)
                var tempFilter = new ParticipantFilterDto
                {
                    FullName = filter.FullName,
                    Email = filter.Email,
                    AuthFullName = filter.AuthFullName,
                    CompanyName = filter.CompanyName,
                    CreateDate = filter.CreateDate,
                    HasLogo = filter.HasLogo,
                    SortBy = filter.SortBy,
                    IsDescending = filter.IsDescending,
                    PageNumber = 1,
                    PageSize = int.MaxValue // Tüm kayıtları al
                };

                var result = await _participantService.FilterPagedAsync(tempFilter);
                var pdfBytes = await _pdfService.GenerateParticipantListPdfAsync(result.Items);
                var fileName = $"Katilimci_Listesi_Filtreli_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "PDF oluşturulurken bir hata oluştu." });
            }
        }


    }
}