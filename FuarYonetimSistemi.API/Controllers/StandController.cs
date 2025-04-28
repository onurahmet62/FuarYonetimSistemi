using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager,SalesPerson")]
    public class StandsController : ControllerBase
    {
        private readonly IStandService _standService;

        public StandsController(IStandService standService)
        {
            _standService = standService;
        }

        // GET api/stands
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stand>>> Get()
        {
            var stands = await _standService.GetAllAsync();
            return Ok(stands);
        }

        // GET api/stands/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Stand>> Get(Guid id)
        {
            var stand = await _standService.GetByIdAsync(id);
            if (stand == null)
            {
                return NotFound();
            }
            return Ok(stand);
        }

        [HttpPost]
        public async Task<ActionResult<Stand>> Post([FromBody] StandCreateDto standCreateDto)
        {
            var createdStand = await _standService.AddAsync(standCreateDto);

            if (createdStand == null)
            {
                return BadRequest("Katılımcı veya fuar bulunamadı.");
            }

            return CreatedAtAction(nameof(Get), new { id = createdStand.Id }, createdStand);
        }

        // PUT api/stands/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Stand>> Put(Guid id, [FromBody] Stand updatedStand)
        {
            var stand = await _standService.UpdateAsync(id, updatedStand);
            if (stand == null)
            {
                return NotFound();
            }
            return Ok(stand);
        }

        // DELETE api/stands/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _standService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        // GET api/stands/search?term={term}
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Stand>>> Search([FromQuery] string term)
        {
            var stands = await _standService.FilterAsync(term);
            return Ok(stands);
        }
    }
}
