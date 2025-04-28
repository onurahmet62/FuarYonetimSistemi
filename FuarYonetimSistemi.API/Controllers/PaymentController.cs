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
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // GET api/payments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> Get()
        {
            var payments = await _paymentService.GetAllAsync();
            return Ok(payments);
        }

        // GET api/payments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> Get(Guid id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
        }

        // GET api/payments/stand/{standId}
        [HttpGet("stand/{standId}")]
        public async Task<ActionResult<IEnumerable<Payment>>> GetByStandId(Guid standId)
        {
            var payments = await _paymentService.GetByStandIdAsync(standId);
            return Ok(payments);
        }

        // GET api/payments/participant/{participantId}
        [HttpGet("participant/{participantId}")]
        public async Task<ActionResult<IEnumerable<Payment>>> GetByParticipantId(Guid participantId)
        {
            var payments = await _paymentService.GetByParticipantIdAsync(participantId);
            return Ok(payments);
        }

        [HttpPost]
        public async Task<ActionResult<Payment>> Post([FromBody] Payment payment)
        {
            try
            {
                // PaymentService üzerinden ödeme kaydediliyor
                var createdPayment = await _paymentService.AddAsync(payment);
                return CreatedAtAction(nameof(Get), new { id = createdPayment.Id }, createdPayment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Stand bulunamadığında hata döndürülür
            }
        }



        // PUT api/payments/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Payment>> Put(Guid id, [FromBody] Payment updatedPayment)
        {
            var payment = await _paymentService.UpdateAsync(id, updatedPayment);
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
        }

        // DELETE api/payments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _paymentService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        // GET api/payments/search?term={term}
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Payment>>> Search([FromQuery] string term)
        {
            var payments = await _paymentService.FilterAsync(term);
            return Ok(payments);
        }
    }
}
