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
    public class StandController : ControllerBase
    {
        private readonly IStandService _standService;

        public StandController(IStandService standService)
        {
            _standService = standService;
        }

        // Stand oluşturma (Admin, Manager ve SalesPerson yapabilir)
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,SalesPerson")]
        public async Task<ActionResult<StandDto>> CreateStandAsync(StandCreateDto standCreateDto)
        {
            try
            {
                var createdStand = await _standService.CreateStandAsync(standCreateDto);
                return Ok(createdStand);  // Stand başarıyla oluşturulduysa geri dönüyoruz
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });  // Hata durumunda mesaj dönüyoruz
            }
        }

        // Stand güncelleme (Admin ve Manager yapabilir)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<StandDto>> UpdateStandAsync(Guid id, StandUpdateDto standUpdateDto)
        {
            if (id != standUpdateDto.Id)
            {
                return BadRequest(new { message = "Stand ID'leri uyuşmuyor." });
            }

            try
            {
                var updatedStand = await _standService.UpdateStandAsync(standUpdateDto);
                return Ok(updatedStand);  // Stand başarıyla güncellendiyse geri dönüyoruz
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });  // Hata durumunda mesaj dönüyoruz
            }
        }

        // Stand silme (Admin yapabilir)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteStandAsync(Guid id)
        {
            try
            {
                var isDeleted = await _standService.DeleteStandAsync(id);
                if (!isDeleted)
                    return NotFound(new { message = "Stand bulunamadı." });

                return Ok(new { message = "Stand başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });  // Hata durumunda mesaj dönüyoruz
            }
        }

        // Bir fuara ait tüm standları listeleme (Admin, Manager ve SalesPerson yapabilir)
        [HttpGet("by-fair/{fairId}")]
        [Authorize(Roles = "Admin,Manager,SalesPerson")]
        public async Task<ActionResult<List<StandDto>>> GetStandsByFairAsync(Guid fairId)
        {
            try
            {
                var stands = await _standService.GetStandsByFairAsync(fairId);
                return Ok(stands);  // Stand'lar başarıyla döndü
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });  // Hata durumunda mesaj dönüyoruz
            }
        }

        // Bir katılımcıya ait tüm standları listeleme (Admin, Manager ve SalesPerson yapabilir)
        [HttpGet("by-participant/{participantId}")]
        [Authorize(Roles = "Admin,Manager,SalesPerson")]
        public async Task<ActionResult<List<StandDto>>> GetStandsByParticipantAsync(Guid participantId)
        {
            try
            {
                var stands = await _standService.GetStandsByParticipantAsync(participantId);
                return Ok(stands);  // Stand'lar başarıyla döndü
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });  // Hata durumunda mesaj dönüyoruz
            }
        }

        // Stand ödeme durumu güncelleme (Admin ve Manager yapabilir)
        [HttpPut("update-payment-status")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<StandDto>> UpdatePaymentStatusAsync(StandPaymentStatusDto paymentStatusDto)
        {
            try
            {
                var updatedStand = await _standService.UpdatePaymentStatusAsync(paymentStatusDto);
                return Ok(updatedStand);  // Ödeme durumu başarıyla güncellendiyse geri dönüyoruz
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });  // Hata durumunda mesaj dönüyoruz
            }
        }

        // Stand detaylarını alma (Admin, Manager ve SalesPerson yapabilir)
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,SalesPerson")]
        public async Task<ActionResult<StandDto>> GetStandByIdAsync(Guid id)
        {
            try
            {
                var stand = await _standService.GetStandByIdAsync(id);
                return Ok(stand);  // Stand başarıyla döndü
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });  // Hata durumunda mesaj dönüyoruz
            }
        }

        [HttpGet("filter")]
        [Authorize(Roles = "Admin,Manager,SalesPerson")]
        public async Task<ActionResult<List<StandDto>>> GetStandsAsync([FromQuery] StandFilterDto filterDto)
        {
            try
            {
                var stands = await _standService.GetStandsAsync(filterDto);
                return Ok(stands);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("upcoming-payments")]
        public async Task<IActionResult> GetStandsWithUpcomingDueDate([FromQuery] int days)
        {
            var stands = await _standService.GetStandsWithUpcomingDueDateAsync(days);
            return Ok(stands);
        }

        [HttpGet("unpaid")]
        public async Task<IActionResult> GetUnpaidStands()
        {
            var unpaidStands = await _standService.GetUnpaidStandsAsync();
            return Ok(unpaidStands);
        }


    }
}
