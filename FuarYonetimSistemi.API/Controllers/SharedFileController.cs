using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SharedFilesController : ControllerBase
    {
        private readonly ISharedFileService _sharedFileService;
        private readonly IWebHostEnvironment _environment;

        public SharedFilesController(ISharedFileService sharedFileService, IWebHostEnvironment environment)
        {
            _sharedFileService = sharedFileService;
            _environment = environment;
        }

        // GET: api/SharedFiles
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var files = await _sharedFileService.GetAllFilesAsync();
            return Ok(files);
        }

        // GET: api/SharedFiles/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var file = await _sharedFileService.GetFileByIdAsync(id);
            if (file == null) return NotFound();
            return Ok(file);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] FileUploadDto fileDto)
        {
            if (fileDto.File == null || fileDto.File.Length == 0)
                return BadRequest("Dosya seçilmedi.");

            var uploadsPath = Path.Combine(_environment.WebRootPath ?? "wwwroot", "uploads");

            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(fileDto.File.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileDto.File.CopyToAsync(stream);
            }

            var dto = new SharedFileCreateDto
            {
                FileName = fileDto.File.FileName,
                FilePath = Path.Combine("uploads", fileName)
            };

            var result = await _sharedFileService.CreateFileAsync(dto);

            return Ok(result);
        }



        // PUT: api/SharedFiles/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SharedFileCreateDto dto)
        {
            var success = await _sharedFileService.UpdateFileAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        // DELETE: api/SharedFiles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _sharedFileService.DeleteFileAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
