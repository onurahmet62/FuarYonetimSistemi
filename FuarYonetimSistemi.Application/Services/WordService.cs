using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// OpenXml namespace'lerini explicit olarak tanımla
using OpenXmlDocument = DocumentFormat.OpenXml.Wordprocessing.Document;
using OpenXmlParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using OpenXmlText = DocumentFormat.OpenXml.Wordprocessing.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace FuarYonetimSistemi.Infrastructure.Services
{
    public class WordService : IWordService
    {
        private readonly ILogger<WordService> _logger;

        public WordService(ILogger<WordService> logger)
        {
            _logger = logger;
        }

        public async Task<byte[]> GenerateParticipantWordAsync(ParticipantDto participant, string fairName)
        {
            return await Task.Run(() =>
            {
                using var memoryStream = new MemoryStream();
                using var document = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document);

                var mainPart = document.AddMainDocumentPart();
                mainPart.Document = new OpenXmlDocument();
                var body = mainPart.Document.AppendChild(new Body());

                // Başlık - Fuar adı
                AddTitle(body, fairName.ToUpper());
                AddTitle(body, "KATILIMCI BİLGİ FORMU");

                // Firma Bilgileri Bölümü
                AddSectionHeader(body, "FİRMA BİLGİLERİ");
                AddParagraph(body, "Aşağıdaki bilgiler fuar kataloğunda kullanılacaktır. Lütfen bilgilerin doğruluğundan emin olunuz.");
                AddEmptyLine(body);

                AddInfoLine(body, "Firma Adı", participant.CompanyName, true);
                AddInfoLine(body, "Adres", participant.Address, true);
                AddInfoLine(body, "Telefon", participant.Phone, true);
                AddInfoLine(body, "E-Posta", participant.Email, false);
                AddInfoLine(body, "Web", participant.Website, false);

                AddEmptyLine(body);
                AddBoldText(body, "Şubeleriniz :");

                // Şubeler listesi
                if (participant.Branches != null && participant.Branches.Any())
                {
                    for (int i = 0; i < participant.Branches.Count && i < 5; i++)
                    {
                        AddNumberedLine(body, i + 1, participant.Branches.ElementAt(i).Name);
                    }
                    // Kalan boş satırlar
                    for (int i = participant.Branches.Count; i < 5; i++)
                    {
                        AddNumberedLine(body, i + 1, "");
                    }
                }
                else
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        AddNumberedLine(body, i, "");
                    }
                }

                // Markalar Bölümü
                AddEmptyLine(body);
                AddSectionHeader(body, "MARKALARINIZ");
                AddParagraph(body, "Firmanız adına tescilli isim ve/veya markanız var mı?");
                AddParagraph(body, "Sözleşme yaptığınız firma isminizin altında markalarınızın yer alabilmesi için marka tescil belgenizi bize iletmenizi rica ediyoruz.");
                AddParagraph(body, "Aksi halde markalarınıza katalogda yer verilemeyecektir.");
                AddParagraph(body, "Sayın katılımcımız, sadece firmanıza ait markalarınızı paylaşmanızı önemle rica ederiz; firmanıza ait olmayan markalara kesinlikle yer verilmeyecektir.");
                AddEmptyLine(body);

                // Markalar listesi
                if (participant.Brands != null && participant.Brands.Any())
                {
                    for (int i = 0; i < participant.Brands.Count && i < 10; i++)
                    {
                        AddNumberedLine(body, i + 1, participant.Brands.ElementAt(i).Name);
                    }
                    // Kalan boş satırlar
                    for (int i = participant.Brands.Count; i < 10; i++)
                    {
                        AddNumberedLine(body, i + 1, "");
                    }
                }
                else
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        AddNumberedLine(body, i, "");
                    }
                }

                // Ürün Grupları Bölümü
                AddEmptyLine(body);
                AddSectionHeader(body, "ÜRÜN GRUPLARI");
                AddParagraph(body, "Değerli Katılımcımız, Fuar kataloğunda, firmanızın ve ürünlerinizin doğru yerlerde yer alması ve hedef alıcı kitleniz tarafından kolaylıkla bulunabilmesi için üreticisi, dağıtıcısı ya da satıcısı olduğunuz ürünlerin yanında bulunan kutuları işaretleyiniz.");
                AddParagraph(body, "Ürün grupları sınıflandırılması:");
                AddEmptyLine(body);

                // Ürün kategorileri
                var productCategories = new[]
                {
                    "Tarımsal Mekanizasyon Ve Teknolojileri",
                    "Hayvansal Üretim Makineleri",
                    "Sera Teknolojileri",
                    "Su ve Sulama Teknolojileri",
                    "Gübreler",
                    "Zirai İlaçlar",
                    "Tohum, Fide, Fidan, Bahçecilik",
                    "Çiçekçilik Ve İlgili Teknolojiler",
                    "Ekolojik Tarım",
                    "Bağcılık Ve Şarapçılık İle İlgili Teknolojiler",
                    "Hayvan Sağlığı Ve Veterinerlik Hizmetleri, İlaçları Ve Ekipmanları",
                    "Sektörel Kamu Kurum Ve Kuruluşları",
                    "Diğer"
                };

                foreach (var category in productCategories)
                {
                    bool isSelected = participant.ProductCategories?.Any(pc => pc.Name.Contains(category)) ?? false;
                    string marker = isSelected ? "******" : "      ";
                    AddParagraph(body, $"{marker}   {category}");
                }

                // Diğer kategorisinde özel ürünler varsa
                var otherProducts = participant.ProductCategories?.Where(pc =>
                    !productCategories.Any(cat => pc.Name.Contains(cat)))?.ToList();
                if (otherProducts?.Any() == true)
                {
                    var otherProductsText = string.Join(" ", otherProducts.Select(p => p.Name));
                    AddParagraph(body, $"******   Diğer {otherProductsText}");
                }

                // Firma Temsilcilik Bölümü
                AddEmptyLine(body);
                AddSectionHeader(body, "FİRMA TEMSİLCİLİK KAYIT FORMU");
                AddParagraph(body, "Temsilciliğini yaptığınız firmaların katalogda yer almaları için firma bilgilerini doldurunuz. Her firma için ayrı ayrı doldurulmalıdır.");
                AddBoldText(body, "Temsilciliğini yaptığınız firma yok ise formu doldurmanıza gerek yoktur.");
                AddEmptyLine(body);

                if (participant.RepresentativeCompanies != null && participant.RepresentativeCompanies.Any())
                {
                    foreach (var rep in participant.RepresentativeCompanies)
                    {
                        AddInfoLine(body, "Temsilci Firma Adı", rep.Name, true);
                        AddInfoLine(body, "Temsilci Firma Ülkesi", rep.Country, true);
                        AddParagraph(body, "Lütfen temsiliciliğini yaptığınız firmanın adres bilgilerini belirtiniz.");
                        AddInfoLine(body, "Adres", rep.Address, true);
                        AddInfoLine(body, "İlçe", rep.District, true);
                        AddInfoLine(body, "Şehir", rep.City, true);
                        AddInfoLine(body, "Telefon", rep.Phone, true);
                        AddInfoLine(body, "E posta", rep.Email, true);
                        AddInfoLine(body, "Web", rep.Website, false);
                        AddEmptyLine(body);
                        AddParagraph(body, "------------------------------");
                        AddEmptyLine(body);
                    }
                }
                else
                {
                    AddInfoLine(body, "Temsilci Firma Adı", "", true);
                    AddInfoLine(body, "Temsilci Firma Ülkesi", "", true);
                    AddParagraph(body, "Lütfen temsiliciliğini yaptığınız firmanın adres bilgilerini belirtiniz.");
                    AddInfoLine(body, "Adres", "", true);
                    AddInfoLine(body, "İlçe", "", true);
                    AddInfoLine(body, "Şehir", "", true);
                    AddInfoLine(body, "Telefon", "", true);
                    AddInfoLine(body, "E posta", "", true);
                    AddInfoLine(body, "Web", "", false);
                }

                // Sergilenecek Ürünler Bölümü
                AddEmptyLine(body);
                AddSectionHeader(body, "FUARDA SERGİLENECEK ÜRÜNLER");
                AddEmptyLine(body);

                if (participant.ExhibitedProducts != null && participant.ExhibitedProducts.Any())
                {
                    foreach (var product in participant.ExhibitedProducts)
                    {
                        AddParagraph(body, $"• {product.Name}");
                    }
                }
                else
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        AddParagraph(body, $"{i}. ");
                    }
                }

                document.Save();
                return memoryStream.ToArray();
            });
        }

        public async Task<byte[]> GenerateAllParticipantsWordAsync(IEnumerable<ParticipantDto> participants, string fairName)
        {
            return await Task.Run(() =>
            {
                using var memoryStream = new MemoryStream();
                using var document = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document);

                var mainPart = document.AddMainDocumentPart();
                mainPart.Document = new OpenXmlDocument();
                var body = mainPart.Document.AppendChild(new Body());

                // Ana başlık
                AddTitle(body, fairName.ToUpper());
                AddTitle(body, "TÜM KATILIMCILAR - BİLGİ FORMLARI");
                AddEmptyLine(body);

                bool isFirst = true;
                foreach (var participant in participants)
                {
                    if (!isFirst)
                    {
                        // Sayfa sonu ekle
                        AddPageBreak(body);
                    }
                    isFirst = false;

                    // Her katılımcı için aynı formatı kullan
                    AddParticipantToDocument(body, participant, fairName);
                }

                document.Save();
                return memoryStream.ToArray();
            });
        }

        private void AddParticipantToDocument(Body body, ParticipantDto participant, string fairName)
        {
            // Katılımcı başlığı
            AddTitle(body, $"KATILIMCI: {participant.CompanyName}");
            AddTitle(body, "KATILIMCI BİLGİ FORMU");

            // Firma Bilgileri (aynı mantık, kısalık için sadece temel bilgiler)
            AddSectionHeader(body, "FİRMA BİLGİLERİ");
            AddInfoLine(body, "Firma Adı", participant.CompanyName, true);
            AddInfoLine(body, "Adres", participant.Address, true);
            AddInfoLine(body, "Telefon", participant.Phone, true);
            AddInfoLine(body, "E-Posta", participant.Email, false);
            AddInfoLine(body, "Web", participant.Website, false);

            // Diğer bölümler de aynı şekilde eklenebilir...
        }

        #region Helper Methods

        private void AddTitle(Body body, string text)
        {
            var paragraph = new OpenXmlParagraph();
            var run = new Run();
            var runProperties = new RunProperties();
            runProperties.Bold = new Bold();
            runProperties.FontSize = new FontSize() { Val = "16" };
            run.RunProperties = runProperties;
            run.AppendChild(new OpenXmlText(text));
            paragraph.AppendChild(run);
            paragraph.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
            body.AppendChild(paragraph);
        }

        private void AddSectionHeader(Body body, string text)
        {
            var paragraph = new OpenXmlParagraph();
            var run = new Run();
            var runProperties = new RunProperties();
            runProperties.Bold = new Bold();
            runProperties.Underline = new Underline() { Val = UnderlineValues.Single };
            run.RunProperties = runProperties;
            run.AppendChild(new OpenXmlText(text));
            paragraph.AppendChild(run);
            body.AppendChild(paragraph);
        }

        private void AddParagraph(Body body, string text)
        {
            var paragraph = new OpenXmlParagraph();
            var run = new Run();
            run.AppendChild(new OpenXmlText(text));
            paragraph.AppendChild(run);
            body.AppendChild(paragraph);
        }

        private void AddBoldText(Body body, string text)
        {
            var paragraph = new OpenXmlParagraph();
            var run = new Run();
            var runProperties = new RunProperties();
            runProperties.Bold = new Bold();
            run.RunProperties = runProperties;
            run.AppendChild(new OpenXmlText(text));
            paragraph.AppendChild(run);
            body.AppendChild(paragraph);
        }

        private void AddInfoLine(Body body, string label, string value, bool isRequired)
        {
            string requiredMark = isRequired ? "*" : "";
            string displayValue = string.IsNullOrEmpty(value) ? "" : value;
            var text = $"{label} : {requiredMark}{displayValue}";
            AddParagraph(body, text);
        }

        private void AddNumberedLine(Body body, int number, string text)
        {
            AddParagraph(body, $"{number}. {text}");
        }

        private void AddEmptyLine(Body body)
        {
            var paragraph = new OpenXmlParagraph();
            var run = new Run();
            run.AppendChild(new OpenXmlText(""));
            paragraph.AppendChild(run);
            body.AppendChild(paragraph);
        }

        private void AddPageBreak(Body body)
        {
            var paragraph = new OpenXmlParagraph();
            var run = new Run();
            run.AppendChild(new Break() { Type = BreakValues.Page });
            paragraph.AppendChild(run);
            body.AppendChild(paragraph);
        }

        #endregion
    }
}