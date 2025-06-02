using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantPdfController : ControllerBase
    {
        private readonly IPdfService _pdfService;
        private readonly IParticipantService _participantService;

        public ParticipantPdfController(IPdfService pdfService, IParticipantService participantService)
        {
            _pdfService = pdfService;
            _participantService = participantService;
        }

        [HttpGet("{participantId:guid}")]
        public async Task<IActionResult> GetParticipantPdf(Guid participantId)
        {
            var participant = await _participantService.GetByIdAsync(participantId);
            if (participant == null)
                return NotFound();

            var pdfBytes = await _pdfService.GenerateParticipantPdfAsync(participant);

            var fileName = $"Katilimci_{participant.FullName?.Replace(" ", "_")}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetParticipantListPdf()
        {
            var participants = await _participantService.GetAllAsync();
            var pdfBytes = await _pdfService.GenerateParticipantListPdfAsync(participants);

            return File(pdfBytes, "application/pdf", "KatilimciListesi.pdf");
        }
    }
}
