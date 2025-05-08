using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager,SalesPerson")]
    public class OfficeExpensesController : ControllerBase
    {
        private readonly IOfficeExpenseService _officeExpenseService;

        public OfficeExpensesController(IOfficeExpenseService officeExpenseService)
        {
            _officeExpenseService = officeExpenseService;
        }

        // Gider Türlerini Listele
        [HttpGet("types")]
        public async Task<IActionResult> GetExpenseTypes()
        {
            var expenseTypes = await _officeExpenseService.GetExpenseTypesAsync();
            return Ok(expenseTypes);
        }

        // Ofis Giderleri Listele
        [HttpGet]
        public async Task<IActionResult> GetAllExpenses()
        {
            var expenses = await _officeExpenseService.GetAllExpensesAsync();
            return Ok(expenses);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OfficeExpenseCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var expense = await _officeExpenseService.CreateAsync(dto);
                return Ok(expense);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Ofis Gideri Sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(Guid id)
        {
            try
            {
                var deletedExpense = await _officeExpenseService.DeleteExpenseAsync(id);
                return Ok(deletedExpense);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Yeni Ofis Gider Türü Ekle
        [HttpPost("types")]
        public async Task<IActionResult> CreateExpenseType([FromBody] OfficeExpenseType expenseType)
        {
            if (expenseType == null)
                return BadRequest("Expense type data is required.");

            var createdExpenseType = await _officeExpenseService.CreateExpenseTypeAsync(expenseType);
            return Ok(createdExpenseType);
        }

        // Ofis Gider Türü Sil
        [HttpDelete("types/{id}")]
        public async Task<IActionResult> DeleteExpenseType(Guid id)
        {
            try
            {
                var deletedExpenseType = await _officeExpenseService.DeleteExpenseTypeAsync(id);
                return Ok(deletedExpenseType);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
