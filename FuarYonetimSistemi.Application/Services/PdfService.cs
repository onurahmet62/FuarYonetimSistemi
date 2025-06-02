using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Infrastructure.Services
{
    public class PdfService : IPdfService
    {
        private readonly IFileService _fileService;
        private readonly ILogger<PdfService> _logger;

        public PdfService(IFileService fileService, ILogger<PdfService> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<byte[]> GenerateParticipantPdfAsync(ParticipantDto participant)
        {
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf, iText.Kernel.Geom.PageSize.A4);
            document.SetMargins(20, 20, 20, 20);

            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var italicFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);

            var title = new Paragraph("KATILIMCI BİLGİLERİ")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(20)
                .SetFont(boldFont);
            document.Add(title);

            if (!string.IsNullOrEmpty(participant.LogoFilePath) && _fileService.FileExists(participant.LogoFilePath))
            {
                try
                {
                    var logoBytes = await _fileService.GetFileContentAsync(participant.LogoFilePath);
                    var imageData = ImageDataFactory.Create(logoBytes);
                    var image = new Image(imageData)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetWidth(150)
                        .SetHeight(100);

                    var logoContainer = new Paragraph().SetTextAlignment(TextAlignment.CENTER);
                    logoContainer.Add(image);
                    document.Add(logoContainer);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Logo yüklenemedi: {ex.Message}");
                }
            }

            document.Add(new Paragraph("\n"));

            var table = new Table(2).SetWidth(UnitValue.CreatePercentValue(100));

            AddTableRow(table, "Ad Soyad:", participant.FullName, boldFont);
            AddTableRow(table, "E-posta:", participant.Email, boldFont);
            AddTableRow(table, "Telefon:", participant.Phone, boldFont);
            AddTableRow(table, "Yetkili Kişi:", participant.AuthFullName, boldFont);
            AddTableRow(table, "Firma Adı:", participant.CompanyName, boldFont);
            AddTableRow(table, "Adres:", participant.Address, boldFont);
            AddTableRow(table, "Web Sitesi:", participant.Website, boldFont);
            AddTableRow(table, "Şubeler:", participant.Branches, boldFont);
            AddTableRow(table, "Kayıt Tarihi:", participant.CreateDate.ToString("dd.MM.yyyy HH:mm"), boldFont);

            if (participant.LogoUploadDate.HasValue)
            {
                AddTableRow(table, "Logo Yükleme Tarihi:", participant.LogoUploadDate.Value.ToString("dd.MM.yyyy HH:mm"), boldFont);
            }

            document.Add(table);

            document.Add(new Paragraph("\n"));

            var footer = new Paragraph("Bu belge sistem tarafından otomatik olarak oluşturulmuştur.")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(10)
                .SetFont(italicFont);
            document.Add(footer);

            document.Close();
            return stream.ToArray();
        }

        public async Task<byte[]> GenerateParticipantListPdfAsync(IEnumerable<ParticipantDto> participants)
        {
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf, iText.Kernel.Geom.PageSize.A4);
            document.SetMargins(20, 20, 20, 20);

            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            var title = new Paragraph("KATILIMCI LİSTESİ")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(20)
                .SetFont(boldFont);
            document.Add(title);

            document.Add(new Paragraph($"Oluşturulma Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm}")
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetFontSize(10));

            document.Add(new Paragraph("\n"));

            var table = new Table(6).SetWidth(UnitValue.CreatePercentValue(100));

            AddTableHeader(table, "Ad Soyad", boldFont);
            AddTableHeader(table, "Firma", boldFont);
            AddTableHeader(table, "E-posta", boldFont);
            AddTableHeader(table, "Telefon", boldFont);
            AddTableHeader(table, "Yetkili", boldFont);
            AddTableHeader(table, "Logo", boldFont);

            foreach (var p in participants)
            {
                table.AddCell(new Cell().Add(new Paragraph(p.FullName ?? "")));
                table.AddCell(new Cell().Add(new Paragraph(p.CompanyName ?? "")));
                table.AddCell(new Cell().Add(new Paragraph(p.Email ?? "")));
                table.AddCell(new Cell().Add(new Paragraph(p.Phone ?? "")));
                table.AddCell(new Cell().Add(new Paragraph(p.AuthFullName ?? "")));
                table.AddCell(new Cell().Add(new Paragraph(!string.IsNullOrEmpty(p.LogoFilePath) ? "✓" : "✗")).SetTextAlignment(TextAlignment.CENTER));
            }

            document.Add(table);

            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph($"Toplam Katılımcı Sayısı: {participants.Count()}")
                .SetFontSize(12)
                .SetFont(boldFont));
            document.Add(new Paragraph($"Logo Yükleyen Katılımcı Sayısı: {participants.Count(p => !string.IsNullOrEmpty(p.LogoFilePath))}")
                .SetFontSize(12));

            document.Close();
            return stream.ToArray();
        }

        private static void AddTableRow(Table table, string label, string value, PdfFont boldFont)
        {
            table.AddCell(new Cell().Add(new Paragraph(label).SetFont(boldFont)).SetPadding(5));
            table.AddCell(new Cell().Add(new Paragraph(value ?? "")).SetPadding(5));
        }

        private static void AddTableHeader(Table table, string header, PdfFont boldFont)
        {
            table.AddCell(new Cell()
                .Add(new Paragraph(header).SetFont(boldFont))
                .SetBackgroundColor(ColorConstants.DARK_GRAY)
                .SetFontColor(ColorConstants.WHITE)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetPadding(5));
        }
    }
}
