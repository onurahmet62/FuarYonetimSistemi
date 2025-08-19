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

                // Markalar tablosu formatı (Word dosyasındaki gibi)
                CreateTableForList(body, participant.Brands?.Select(b => b.Name).ToList(), 10);

                // Ürün Grupları Bölümü
                AddEmptyLine(body);
                AddSectionHeader(body, "ÜRÜN GRUPLARI");
                AddParagraph(body, "Değerli Katılımcımız, Fuar kataloğunda, firmanızın ve ürünlerinizin doğru yerlerde yer alması ve hedef alıcı kitleniz tarafından kolaylıkla bulunabilmesi için üreticisi, dağıtıcısı ya da satıcısı olduğunuz ürünlerin yanında bulunan kutuları işaretleyiniz.");
                AddParagraph(body, "Ürün grupları sınıflandırılması:");
                AddEmptyLine(body);

                // Ürün Grupları - Sadece katılımcının ProductCategory tablosundaki veriler gösterilecek
                if (participant.ProductCategories != null && participant.ProductCategories.Any())
                {
                    foreach (var category in participant.ProductCategories)
                    {
                        AddParagraph(body, $"******   {category.Name}");
                    }
                }
                else
                {
                    // Hiç kategori seçilmemişse boş bırak
                    AddParagraph(body, "Seçili kategori yok.");
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

                // Sergilenecek Ürünler - ExhibitedProducts tablosundan getir
                if (participant.ExhibitedProducts != null && participant.ExhibitedProducts.Any())
                {
                    int productIndex = 1;
                    foreach (var product in participant.ExhibitedProducts)
                    {
                        AddParagraph(body, $"{productIndex}. {product.Name}");
                        productIndex++;
                    }

                    // Kalan boş satırları ekle (toplam 15 satır olacak şekilde)
                    for (int i = productIndex; i <= 15; i++)
                    {
                        AddParagraph(body, $"{i}. ");
                    }
                }
                else
                {
                    // Hiç ürün yoksa 15 boş satır
                    for (int i = 1; i <= 15; i++)
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

        private void CreateTableForList(Body body, List<string> items, int maxRows)
        {
            // Tablo oluştur
            var table = new Table();

            // Tablo özellikleri
            var tableProperties = new TableProperties(
                new TableBorders(
                    new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                    new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                    new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                    new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                    new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                    new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 }
                )
            );
            table.AppendChild(tableProperties);

            // Satırları oluştur (2 sütunlu tablo)
            for (int i = 0; i < maxRows; i += 2)
            {
                var tableRow = new TableRow();

                // Sol sütun
                var leftCell = new TableCell();
                var leftCellPara = new OpenXmlParagraph();
                var leftRun = new Run();

                string leftContent = "";
                if (i < maxRows)
                {
                    if (items != null && i < items.Count && !string.IsNullOrEmpty(items[i]))
                        leftContent = $"{i + 1}    {items[i]}";
                    else
                        leftContent = $"{i + 1}    ";
                }

                leftRun.AppendChild(new OpenXmlText(leftContent));
                leftCellPara.AppendChild(leftRun);
                leftCell.AppendChild(leftCellPara);
                leftCell.AppendChild(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "50" }));
                tableRow.AppendChild(leftCell);

                // Sağ sütun
                var rightCell = new TableCell();
                var rightCellPara = new OpenXmlParagraph();
                var rightRun = new Run();

                string rightContent = "";
                if (i + 1 < maxRows)
                {
                    if (items != null && i + 1 < items.Count && !string.IsNullOrEmpty(items[i + 1]))
                        rightContent = $"{i + 2}    {items[i + 1]}";
                    else
                        rightContent = $"{i + 2}    ";
                }

                rightRun.AppendChild(new OpenXmlText(rightContent));
                rightCellPara.AppendChild(rightRun);
                rightCell.AppendChild(rightCellPara);
                rightCell.AppendChild(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "50" }));
                tableRow.AppendChild(rightCell);

                table.AppendChild(tableRow);
            }

            body.AppendChild(table);
        }

        #endregion
    }
}