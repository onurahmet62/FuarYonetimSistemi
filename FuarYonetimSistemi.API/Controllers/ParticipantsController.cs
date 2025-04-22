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
    }

}
