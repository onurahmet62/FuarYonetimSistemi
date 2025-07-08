using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
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

            // Başlık
            var title = new Paragraph("KATILIMCI BİLGİ FORMU")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(16)
                .SetFont(boldFont);
            document.Add(title);
            document.Add(new Paragraph("\nFİRMA BİLGİLERİ\n")
                .SetFont(boldFont));

            // Firma Bilgileri
            document.Add(new Paragraph($"Firma Adı : *{participant.CompanyName}"));
            document.Add(new Paragraph($"Adres : *{participant.Address}"));
            document.Add(new Paragraph($"Telefon : *{participant.Phone}"));
            document.Add(new Paragraph($"E-Posta : {participant.Email}"));
            document.Add(new Paragraph($"Web : {participant.Website}"));

            document.Add(new Paragraph("\nŞubeleriniz :").SetFont(boldFont));
            if (participant.Branches != null && participant.Branches.Any())
            {
                int i = 1;
                foreach (var branch in participant.Branches)
                    document.Add(new Paragraph($"{i}.\t{branch.Name}"));
                for (int j = participant.Branches.Count + 1; j <= 5; j++)
                    document.Add(new Paragraph($"{j}.\t"));
            }
            else
            {
                for (int j = 1; j <= 5; j++)
                    document.Add(new Paragraph($"{j}.\t"));
            }

            document.Add(new Paragraph("\nMARKALARINIZ\n").SetFont(boldFont));
            document.Add(new Paragraph("Firmanız adına tescilli isim ve/veya markanız var mı?"));
            document.Add(new Paragraph("Sözleşme yaptığınız firma isminizin altında markalarınızın yer alabilmesi için marka tescil belgenizi bize iletmenizi rica ediyoruz."));
            document.Add(new Paragraph("Aksi halde markalarınıza katalogda yer verilemeyecektir."));
            document.Add(new Paragraph("Sayın katılımcımız, sadece firmanıza ait markalarınızı paylaşmanızı önemle rica ederiz; firmanıza ait olmayan markalara kesinlikle yer verilmeyecektir."));

            if (participant.Brands != null && participant.Brands.Any())
            {
                int i = 1;
                foreach (var brand in participant.Brands)
                    document.Add(new Paragraph($"{i}.\t{brand.Name}"));
                for (int j = participant.Brands.Count + 1; j <= 10; j++)
                    document.Add(new Paragraph($"{j}.\t"));
            }
            else
            {
                for (int j = 1; j <= 10; j++)
                    document.Add(new Paragraph($"{j}.\t"));
            }

            document.Add(new Paragraph("\nÜRÜN GRUPLARI\n").SetFont(boldFont));
            if (participant.ProductCategories != null && participant.ProductCategories.Any())
            {
                foreach (var pc in participant.ProductCategories)
                    document.Add(new Paragraph($"******\t{pc.Name}"));
            }

            document.Add(new Paragraph("\nFİRMA TEMSİLCİLİK KAYIT FORMU\n").SetFont(boldFont));
            if (participant.RepresentativeCompanies != null && participant.RepresentativeCompanies.Any())
            {
                foreach (var rep in participant.RepresentativeCompanies)
                {
                    document.Add(new Paragraph($"Temsilci Firma Adı : *{rep.Name}"));
                    document.Add(new Paragraph($"Temsilci Firma Ülkesi : *{rep.Country}"));
                    document.Add(new Paragraph($"Adres : *{rep.Address}"));
                    document.Add(new Paragraph($"İlçe : *{rep.District}"));
                    document.Add(new Paragraph($"Şehir : *{rep.City}"));
                    document.Add(new Paragraph($"Telefon : *{rep.Phone}"));
                    document.Add(new Paragraph($"E-Posta : *{rep.Email}"));
                    document.Add(new Paragraph($"Web : {rep.Website}"));
                    document.Add(new Paragraph("------------------------------"));
                }
            }
            else
            {
                document.Add(new Paragraph("Temsilciliğini yaptığınız firma yok."));
            }

            document.Add(new Paragraph("\nFUARDA SERGİLENECEK ÜRÜNLER\n").SetFont(boldFont));
            if (participant.ExhibitedProducts != null && participant.ExhibitedProducts.Any())
            {
                int i = 1;
                foreach (var item in participant.ExhibitedProducts)
                    document.Add(new Paragraph($"{i}.\t{item.Name}"));
            }

            // Alt bilgi
            document.Add(new Paragraph("\nBu belge sistem tarafından otomatik olarak oluşturulmuştur.")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(10)
                .SetFont(italicFont));

            document.Close();
            return stream.ToArray();
        }

        public async Task<byte[]> GenerateParticipantListPdfAsync(IEnumerable<ParticipantDto> participants)
        {
            // bu metot aynı şekilde kalabilir
            // yukarıda güncellenen metod zaten detaylı katılımcı bilgilerini içeriyor
            return await Task.FromResult(Array.Empty<byte>()); // placeholder
        }
    }
}
