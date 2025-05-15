using ClosedXML.Excel;
using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FuarYonetimSistemi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager,SalesPerson")]
    public class ParticipantsController : ControllerBase
    {
        private readonly IParticipantService _participantService;

        public ParticipantsController(IParticipantService participantService)
        {
            _participantService = participantService;
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
        public async Task<IActionResult> Create(CreateParticipantDto dto)
        {
            var participant = await _participantService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = participant.Id }, participant);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateParticipantDto dto)
        {
            var participant = await _participantService.UpdateAsync(id, dto);
            if (participant == null)
                return NotFound();

            return Ok(participant);
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

        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportParticipantsToExcel()
        {
            var participants = await _participantService.GetAllAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Participants");

            worksheet.Cell(1, 1).Value = "Ad Soyad";
            worksheet.Cell(1, 2).Value = "Firma";
            worksheet.Cell(1, 3).Value = "Telefon";
            worksheet.Cell(1, 4).Value = "Email";

            int row = 2;
            foreach (var p in participants)
            {
                worksheet.Cell(row, 1).Value = p.FullName;
                worksheet.Cell(row, 2).Value = p.CompanyName;
                worksheet.Cell(row, 3).Value = p.Phone;
                worksheet.Cell(row, 4).Value = p.Email;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Participants.xlsx");
        }
    }

}
