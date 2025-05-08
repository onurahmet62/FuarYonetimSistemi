using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FairExpensesController : ControllerBase
    {
        private readonly IFairExpenseService _fairExpenseService;

        public FairExpensesController(IFairExpenseService fairExpenseService)
        {
            _fairExpenseService = fairExpenseService;
        }

        // Fuar giderlerini listele
        [HttpGet("fair/{fairId}")]
        public async Task<IActionResult> GetFairExpenses(Guid fairId)
        {
            var expenses = await _fairExpenseService.GetFairExpensesAsync(fairId);
            return Ok(expenses);
        }

        // Fuar gideri oluştur
        [HttpPost]
        public async Task<IActionResult> CreateFairExpense([FromBody] FairExpenseCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var expense = await _fairExpenseService.CreateFairExpenseAsync(dto);
                return Ok(expense);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Fuar gideri sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFairExpense(Guid id)
        {
            try
            {
                var deletedExpense = await _fairExpenseService.DeleteFairExpenseAsync(id);
                return Ok(deletedExpense);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Fuar gider türlerini listele
        [HttpGet("expense-types")]
        public async Task<IActionResult> GetExpenseTypes()
        {
            var expenseTypes = await _fairExpenseService.GetExpenseTypesAsync();
            return Ok(expenseTypes);
        }

        // Fuar gider türü oluştur
        [HttpPost("expense-type")]
        public async Task<IActionResult> CreateExpenseType([FromBody] FairExpenseTypeCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdType = await _fairExpenseService.CreateExpenseTypeAsync(dto);
                return Ok(createdType);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Fuar gider türü sil
        [HttpDelete("expense-type/{id}")]
        public async Task<IActionResult> DeleteExpenseType(Guid id)
        {
            try
            {
                var deletedType = await _fairExpenseService.DeleteExpenseTypeAsync(id);
                return Ok(deletedType);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
