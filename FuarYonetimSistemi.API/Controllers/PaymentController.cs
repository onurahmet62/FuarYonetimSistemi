using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FuarYonetimSistemi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _paymentService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id) => Ok(await _paymentService.GetByIdAsync(id));

        [HttpGet("participant/{participantId}")]
        public async Task<IActionResult> GetByParticipant(Guid participantId) => Ok(await _paymentService.GetByParticipantIdAsync(participantId));

        [HttpGet("stand/{standId}")]
        public async Task<IActionResult> GetByStand(Guid standId) => Ok(await _paymentService.GetByStandIdAsync(standId));

        [HttpPost]
        public async Task<IActionResult> Create(Payment payment) => Ok(await _paymentService.AddAsync(payment));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _paymentService.DeleteAsync(id);
            return NoContent();
        }
    }
}
