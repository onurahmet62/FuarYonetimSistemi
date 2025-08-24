using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı");
            }
            return userId;
        }

        /// <summary>
        /// Tüm standları getir
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stand>>> Get()
        {
            try
            {
                var stands = await _standService.GetAllAsync();

                // Admin değilse sadece kendi standlarını görebilir
                if (!User.IsInRole("Admin"))
                {
                    var currentUserId = GetCurrentUserId();
                    stands = stands.Where(s => s.SalesRepresentativeId == currentUserId);
                }

                return Ok(stands);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Stand detaylarını getir
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Stand>> Get(Guid id)
        {
            try
            {
                var stand = await _standService.GetByIdAsync(id);
                if (stand == null)
                {
                    return NotFound("Stand bulunamadı");
                }

                // Admin değilse sadece kendi standını görebilir
                if (!User.IsInRole("Admin"))
                {
                    var currentUserId = GetCurrentUserId();
                    if (stand.SalesRepresentativeId != currentUserId)
                    {
                        return Forbid("Bu standı görme yetkiniz yok");
                    }
                }

                return Ok(stand);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Yeni stand oluştur
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Stand>> Post([FromBody] StandCreateDto standCreateDto)
        {
            try
            {
                // Eğer SalesRepresentativeId belirtilmemişse, current user'ı ata
                if (!standCreateDto.SalesRepresentativeId.HasValue)
                {
                    standCreateDto.SalesRepresentativeId = GetCurrentUserId();
                }
                else
                {
                    // Admin değilse sadece kendisini sales rep olarak atayabilir
                    if (!User.IsInRole("Admin") && standCreateDto.SalesRepresentativeId != GetCurrentUserId())
                    {
                        return Forbid("Sadece kendinizi satış temsilcisi olarak atayabilirsiniz");
                    }
                }

                var createdStand = await _standService.AddAsync(standCreateDto);

                if (createdStand == null)
                {
                    return BadRequest("Katılımcı, fuar veya satış temsilcisi bulunamadı.");
                }

                return CreatedAtAction(nameof(Get), new { id = createdStand.Id }, createdStand);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Stand güncelle
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<Stand>> Put(Guid id, [FromBody] Stand updatedStand)
        {
            try
            {
                // Stand'ın mevcut halini kontrol et
                var existingStand = await _standService.GetByIdAsync(id);
                if (existingStand == null)
                {
                    return NotFound("Stand bulunamadı");
                }

                // Admin değilse sadece kendi standını güncelleyebilir
                if (!User.IsInRole("Admin"))
                {
                    var currentUserId = GetCurrentUserId();
                    if (existingStand.SalesRepresentativeId != currentUserId)
                    {
                        return Forbid("Bu standı güncelleme yetkiniz yok");
                    }

                    // Admin değilse SalesRepresentativeId değiştiremez
                    if (updatedStand.SalesRepresentativeId != existingStand.SalesRepresentativeId)
                    {
                        return Forbid("Satış temsilcisini değiştirme yetkiniz yok");
                    }
                }

                var stand = await _standService.UpdateAsync(id, updatedStand);
                if (stand == null)
                {
                    return NotFound("Stand güncellenemedi");
                }
                return Ok(stand);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Stand sil (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var existingStand = await _standService.GetByIdAsync(id);
                if (existingStand == null)
                {
                    return NotFound("Stand bulunamadı");
                }

                // Admin değilse sadece kendi standını silebilir
                if (!User.IsInRole("Admin"))
                {
                    var currentUserId = GetCurrentUserId();
                    if (existingStand.SalesRepresentativeId != currentUserId)
                    {
                        return Forbid("Bu standı silme yetkiniz yok");
                    }
                }

                var result = await _standService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound("Stand bulunamadı");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Filtrelenmiş stand listesi
        /// </summary>
        [HttpPost("filter")]
        public async Task<IActionResult> GetFilteredStands([FromBody] StandFilterRequestDto request)
        {
            try
            {
                var stands = await _standService.GetSortedAsync(request);

                // Admin değilse sadece kendi standlarını görebilir
                if (!User.IsInRole("Admin"))
                {
                    var currentUserId = GetCurrentUserId();
                    stands = stands.Where(s => s.SalesRepresentativeId == currentUserId);
                }

                return Ok(stands);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Vadesi yaklaşan standları getir
        /// </summary>
        [HttpGet("due-in/{days}")]
        public async Task<ActionResult<IEnumerable<Stand>>> GetStandsDueInDays(int days)
        {
            try
            {
                var allDueStands = await _standService.GetStandsDueInDaysAsync(days);

                // Admin değilse sadece kendi standlarını görebilir
                if (!User.IsInRole("Admin"))
                {
                    var currentUserId = GetCurrentUserId();
                    allDueStands = allDueStands.Where(s => s.SalesRepresentativeId == currentUserId);
                }

                return Ok(allDueStands);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Satış temsilcisine ait standları getir
        /// </summary>
        [HttpGet("by-sales-rep/{salesRepId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<Stand>>> GetBySalesRepresentative(Guid salesRepId)
        {
            try
            {
                var stands = await _standService.GetBySalesRepresentativeIdAsync(salesRepId);
                return Ok(stands);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Katılımcıya ait standları getir
        /// </summary>
        [HttpGet("by-participant/{participantId}")]
        public async Task<ActionResult<IEnumerable<Stand>>> GetByParticipant(Guid participantId)
        {
            try
            {
                var allStands = await _standService.GetByParticipantIdAsync(participantId);

                // Admin değilse sadece kendi standlarını görebilir
                if (!User.IsInRole("Admin"))
                {
                    var currentUserId = GetCurrentUserId();
                    allStands = allStands.Where(s => s.SalesRepresentativeId == currentUserId);
                }

                return Ok(allStands);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Fuara ait standları getir
        /// </summary>
        [HttpGet("by-fair/{fairId}")]
        public async Task<ActionResult<IEnumerable<Stand>>> GetByFair(Guid fairId)
        {
            try
            {
                var allStands = await _standService.GetByFairIdAsync(fairId);

                // Admin değilse sadece kendi standlarını görebilir
                if (!User.IsInRole("Admin"))
                {
                    var currentUserId = GetCurrentUserId();
                    allStands = allStands.Where(s => s.SalesRepresentativeId == currentUserId);
                }

                return Ok(allStands);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Stand arama
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Stand>>> Search([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest("Arama terimi boş olamaz");
                }

                var allSearchResults = await _standService.FilterAsync(searchTerm);

                // Admin değilse sadece kendi standlarını görebilir
                if (!User.IsInRole("Admin"))
                {
                    var currentUserId = GetCurrentUserId();
                    allSearchResults = allSearchResults.Where(s => s.SalesRepresentativeId == currentUserId);
                }

                return Ok(allSearchResults);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Stand özet bilgileri (dashboard için)
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult> GetStandsSummary()
        {
            try
            {
                var allStands = await _standService.GetAllAsync();

                // Admin değilse sadece kendi standları
                if (!User.IsInRole("Admin"))
                {
                    var currentUserId = GetCurrentUserId();
                    allStands = allStands.Where(s => s.SalesRepresentativeId == currentUserId);
                }

                var currentDate = DateTime.Now;

                var summary = new
                {
                    TotalStands = allStands.Count(),
                    TotalRevenue = allStands.Sum(s => s.TotalAmountWithVAT ?? 0),
                    TotalOutstanding = allStands.Sum(s => s.Balance ?? 0),
                    OverdueStands = allStands.Count(s => s.ActualDueDate.HasValue &&
                        s.ActualDueDate.Value < currentDate && (s.Balance ?? 0) > 0),
                    DueSoonStands = allStands.Count(s => s.ActualDueDate.HasValue &&
                        s.ActualDueDate.Value >= currentDate &&
                        s.ActualDueDate.Value <= currentDate.AddDays(30) && (s.Balance ?? 0) > 0),
                    StandsThisMonth = allStands.Count(s => s.ContractDate.HasValue &&
                        s.ContractDate.Value.Year == currentDate.Year &&
                        s.ContractDate.Value.Month == currentDate.Month)
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}