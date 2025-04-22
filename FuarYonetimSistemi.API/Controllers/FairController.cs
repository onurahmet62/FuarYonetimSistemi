using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FuarYonetimSistemi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FairController : ControllerBase
    {
        private readonly IFairService _fairService;
        private readonly ICategoryService _categoryService;

        public FairController(IFairService fairService, ICategoryService categoryService)
        {
            _fairService = fairService;
            _categoryService = categoryService;
        }

        // GET: api/Fair
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,SalesPerson,Customer")]
        public async Task<IActionResult> GetAllFairs()
        {
            var fairs = await _fairService.GetAllFairsAsync();
            return Ok(fairs);
        }

        // GET: api/Fair/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,SalesPerson,Customer")]
        public async Task<IActionResult> GetFairById(Guid id)
        {
            var fair = await _fairService.GetFairByIdAsync(id);
            if (fair == null)
                return NotFound();

            return Ok(fair);
        }

        // POST: api/Fair
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateFair([FromBody] FairCreateDto fairDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kategori kontrolü
            if (fairDto.CategoryId == null && string.IsNullOrWhiteSpace(fairDto.NewCategoryName))
            {
                return BadRequest("Kategori seçilmeli veya yeni bir kategori adı girilmelidir.");
            }

            // Yeni kategori oluşturulacaksa
            if (!string.IsNullOrWhiteSpace(fairDto.NewCategoryName))
            {
                var existingCategory = await _categoryService.GetCategoryByNameAsync(fairDto.NewCategoryName);
                if (existingCategory != null)
                {
                    fairDto.CategoryId = existingCategory.Id;
                }
                else
                {
                    var newCategory = await _categoryService.CreateCategoryAsync(fairDto.NewCategoryName);
                    fairDto.CategoryId = newCategory.Id;
                }
            }

            var createdFair = await _fairService.CreateFairAsync(fairDto);
            return CreatedAtAction(nameof(GetFairById), new { id = createdFair.Id }, createdFair);
        }

        // DELETE: api/Fair/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteFair(Guid id)
        {
            var result = await _fairService.DeleteFairAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }


        // POST: api/Fair/filter
        [HttpPost("filter")]
        [Authorize(Roles = "Admin,Manager,SalesPerson,Customer")]
        public async Task<IActionResult> GetFilteredFairs([FromBody] FairFilterDto filterDto)
        {
            var (fairs, totalCount) = await _fairService.GetFilteredFairsAsync(filterDto);
            return Ok(new { fairs, totalCount });
        }

        // POST: api/Fair/export
        [HttpPost("export")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ExportFairsToExcel([FromBody] FairFilterDto filterDto)
        {
            var excelBytes = await _fairService.ExportFairsToExcelAsync(filterDto);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Fuarlar.xlsx");
        }

    }

}
