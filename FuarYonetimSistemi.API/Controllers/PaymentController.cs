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
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager,SalesPerson")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>Tüm ödemeleri getirir.</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetAll()
        {
            var payments = await _paymentService.GetAllAsync();
            return Ok(payments);
        }

        /// <summary>Filtreleme, sıralama ve sayfalama ile ödeme listesi.</summary>
        [HttpPost("filter")]
        public async Task<ActionResult<PagedResult<Payment>>> GetFiltered([FromBody] PaymentFilterDto filter)
        {
            var payments = await _paymentService.GetFilteredAsync(filter);
            return Ok(payments);
        }

        /// <summary>Ödeme ID'sine göre ödeme bilgisi getirir.</summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetById(Guid id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null) return NotFound();
            return Ok(payment);
        }

        /// <summary>Belirli bir ödeme ile birlikte Stand ve Fuar bilgilerini getirir.</summary>
        [HttpGet("{id}/detail")]
        public async Task<ActionResult<PaymentWithStandAndFairDto>> GetWithStandAndFair(Guid id)
        {
            var result = await _paymentService.GetWithStandAndFairAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>Yeni ödeme oluşturur.</summary>
        [HttpPost]
        public async Task<ActionResult<Payment>> Create([FromBody] PaymentCreateDto dto)
        {
            try
            {
                var created = await _paymentService.AddAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Ödeme güncelleme.</summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<Payment>> Update(Guid id, [FromBody] PaymentUpdateDto dto)
        {
            try
            {
                var updated = await _paymentService.UpdateAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Ödeme silme (soft delete).</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _paymentService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
