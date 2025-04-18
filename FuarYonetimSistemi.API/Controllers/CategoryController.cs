using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FuarYonetimSistemi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/Category
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,SalesPerson,Customer")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        // GET: api/Category/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,SalesPerson,Customer")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };

            return Ok(categoryDto);
        }

        // POST: api/Category
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingCategory = await _categoryService.GetCategoryByNameAsync(categoryDto.Name);
            if (existingCategory != null)
                return Conflict("Bu kategori adı zaten mevcut.");

            var newCategory = await _categoryService.CreateCategoryAsync(categoryDto.Name);
            var resultDto = new CategoryDto
            {
                Id = newCategory.Id,
                Name = newCategory.Name
            };

            return CreatedAtAction(nameof(GetCategoryById), new { id = resultDto.Id }, resultDto);
        }
    }
}
