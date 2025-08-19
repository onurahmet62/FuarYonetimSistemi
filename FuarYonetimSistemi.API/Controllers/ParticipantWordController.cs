using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantWordController : ControllerBase
    {
        private readonly IWordService _wordService;
        private readonly IParticipantService _participantService;

        public ParticipantWordController(
            IWordService wordService,
            IParticipantService participantService)
        {
            _wordService = wordService;
            _participantService = participantService;
        }

        /// <summary>
        /// Belirli bir katılımcının Word belgesini oluştur
        /// </summary>
        /// <param name="participantId">Katılımcı ID'si</param>
        /// <param name="fairName">Fuar adı (opsiyonel)</param>
        /// <returns>Word belgesi</returns>
        [HttpGet("{participantId:guid}")]
        public async Task<IActionResult> GetParticipantWord(Guid participantId, [FromQuery] string fairName = null)
        {
            try
            {
                // Katılımcı bilgilerini getir
                var participant = await _participantService.GetByIdAsync(participantId);
                if (participant == null)
                    return NotFound($"Katılımcı bulunamadı. ID: {participantId}");

                // Fuar adını belirle
                string finalFairName = fairName ?? "MEZOPOTOMYA TARIM HAYVANCILIK FUARI";

                // Word belgesini oluştur
                var wordBytes = await _wordService.GenerateParticipantWordAsync(participant, finalFairName);

                // Dosya adı oluştur
                var fileName = $"Katilimci_{participant.CompanyName?.Replace(" ", "_").Replace("/", "_")}_{DateTime.Now:yyyyMMdd}.docx";

                return File(wordBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Belge oluşturulurken hata oluştu: {ex.Message}");
            }
        }

        /// <summary>
        /// Tüm katılımcıların Word belgelerini oluştur
        /// </summary>
        /// <param name="fairName">Fuar adı (opsiyonel)</param>
        /// <returns>Tüm katılımcıları içeren Word belgesi</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllParticipantsWord([FromQuery] string fairName = null)
        {
            try
            {
                // Tüm katılımcıları getir
                var participants = await _participantService.GetAllAsync();
                if (!participants.Any())
                    return NotFound("Hiçbir katılımcı bulunamadı.");

                // Fuar adını belirle
                string finalFairName = fairName ?? "MEZOPOTOMYA TARIM HAYVANCILIK FUARI";

                // Word belgesini oluştur
                var wordBytes = await _wordService.GenerateAllParticipantsWordAsync(participants, finalFairName);

                // Dosya adı oluştur
                var fileName = $"TumKatilimcilar_{finalFairName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.docx";

                return File(wordBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Belge oluşturulurken hata oluştu: {ex.Message}");
            }
        }

        // <summary>
        /// Belirli bir fuara katılan katılımcıların Word belgelerini oluştur
        /// </summary>
        /// <param name="fairId">Fuar ID'si</param>
        /// <returns>Fuardaki katılımcıları içeren Word belgesi</returns>
        [HttpGet("fair/{fairId:guid}")]
        public async Task<IActionResult> GetFairParticipantsWord(Guid fairId)
        {
            try
            {
                // Fuar bilgilerini al (fair service dependency'si olmadığı için sadece ID kullanacağız)
                // Eğer fuar adına ihtiyaç varsa, ayrı bir endpoint'ten alınabilir

                // Fuardaki katılımcıları getir
                var participants = await _participantService.GetByFairIdAsync(fairId);
                if (!participants.Any())
                    return NotFound($"Bu fuarda katılımcı bulunamadı. Fuar ID: {fairId}");

                // Varsayılan fuar adı (isteğe bağlı olarak fair name parametresi eklenebilir)
                string fairName = $"FUAR - {fairId}";

                // Word belgesini oluştur
                var wordBytes = await _wordService.GenerateAllParticipantsWordAsync(participants, fairName);

                // Dosya adı oluştur
                var fileName = $"FuarKatilimcilari_{fairId}_{DateTime.Now:yyyyMMdd}.docx";

                return File(wordBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Belge oluşturulurken hata oluştu: {ex.Message}");
            }
        }

        /// <summary>
        /// Belirli bir fuara katılan katılımcıların Word belgelerini oluştur (fuar adıyla)
        /// </summary>
        /// <param name="fairId">Fuar ID'si</param>
        /// <param name="fairName">Fuar adı</param>
        /// <returns>Fuardaki katılımcıları içeren Word belgesi</returns>
        [HttpGet("fair/{fairId:guid}/with-name")]
        public async Task<IActionResult> GetFairParticipantsWordWithName(Guid fairId, [FromQuery] string fairName)
        {
            try
            {
                // Fuardaki katılımcıları getir
                var participants = await _participantService.GetByFairIdAsync(fairId);
                if (!participants.Any())
                    return NotFound($"Bu fuarda katılımcı bulunamadı. Fuar ID: {fairId}");

                // Fuar adını belirle
                string finalFairName = string.IsNullOrWhiteSpace(fairName)
                    ? $"FUAR - {fairId}"
                    : fairName;

                // Word belgesini oluştur
                var wordBytes = await _wordService.GenerateAllParticipantsWordAsync(participants, finalFairName);

                // Dosya adı oluştur
                var fileName = $"FuarKatilimcilari_{finalFairName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.docx";

                return File(wordBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Belge oluşturulurken hata oluştu: {ex.Message}");
            }
        }

        /// <summary>
        /// Test endpoint - Basit Word belgesi oluştur
        /// </summary>
        [HttpGet("test")]
        public async Task<IActionResult> TestWordGeneration()
        {
            try
            {
                // Test katılımcısı oluştur
                var testParticipant = new ParticipantDto
                {
                    Id = Guid.NewGuid(),
                    CompanyName = "Test Firma A.Ş.",
                    FullName = "Test Yetkili",
                    Email = "test@firma.com",
                    Phone = "0555-123-4567",
                    Address = "Test Mahallesi Test Caddesi No:1 Test/İstanbul",
                    Website = "www.testfirma.com"
                };

                var wordBytes = await _wordService.GenerateParticipantWordAsync(testParticipant, "TEST FUARI");
                var fileName = $"Test_Word_Belgesi_{DateTime.Now:yyyyMMddHHmmss}.docx";

                return File(wordBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Test belgesi oluşturulurken hata oluştu: {ex.Message}");
            }
        }
    }
}