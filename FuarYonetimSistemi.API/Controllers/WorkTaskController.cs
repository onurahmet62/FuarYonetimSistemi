using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using WorkTaskStatus = FuarYonetimSistemi.Domain.Enums.WorkTaskStatus;

namespace FuarYonetimSistemi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkTaskController : ControllerBase
    {
        private readonly IWorkTaskService _workTaskService;

        public WorkTaskController(IWorkTaskService workTaskService)
        {
            _workTaskService = workTaskService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");
            return userId;
        }

        private UserRole GetCurrentUserRole()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(roleClaim) || !Enum.TryParse<UserRole>(roleClaim, out var role))
                throw new UnauthorizedAccessException("Kullanıcı rolü bulunamadı.");
            return role;
        }

        // WorkTask CRUD Operations

        /// <summary>Yeni görev oluştur (Sadece Manager'lar)</summary>
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateWorkTask([FromBody] WorkTaskCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var currentUserId = GetCurrentUserId();
                var task = await _workTaskService.CreateWorkTaskAsync(dto, currentUserId);
                return CreatedAtAction(nameof(GetWorkTask), new { id = task.Id }, task);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Görev oluşturulurken bir hata oluştu.", detail = ex.Message });
            }
        }

        /// <summary>Görev detayı getir</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkTask(Guid id)
        {
            try
            {
                var task = await _workTaskService.GetWorkTaskByIdAsync(id);
                if (task == null)
                    return NotFound(new { message = "Görev bulunamadı." });

                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                // Yetki kontrolü: SalesPerson sadece kendi task'larını görebilir
                if (currentUserRole == UserRole.SalesPerson &&
                    task.AssignedToUserId != currentUserId &&
                    task.CreatedByUserId != currentUserId)
                {
                    return Forbid("Bu görevi görme yetkiniz yok.");
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Görev alınırken bir hata oluştu.", detail = ex.Message });
            }
        }

        /// <summary>Görev güncelle</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkTask(Guid id, [FromBody] WorkTaskUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                dto.Id = id;
                var currentUserId = GetCurrentUserId();
                var task = await _workTaskService.UpdateWorkTaskAsync(dto, currentUserId);

                if (task == null)
                    return NotFound(new { message = "Görev bulunamadı." });

                return Ok(task);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Görev güncellenirken bir hata oluştu.", detail = ex.Message });
            }
        }

        /// <summary>Görev sil (Sadece Manager'lar)</summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteWorkTask(Guid id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var result = await _workTaskService.DeleteWorkTaskAsync(id, currentUserId);

                if (!result)
                    return NotFound(new { message = "Görev bulunamadı." });

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Görev silinirken bir hata oluştu.", detail = ex.Message });
            }
        }

        // WorkTask Status Management

        /// <summary>Görev durumu güncelle</summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateWorkTaskStatus(Guid id, [FromBody] WorkTaskStatusUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                dto.WorkTaskId = id;
                var currentUserId = GetCurrentUserId();
                var task = await _workTaskService.UpdateWorkTaskStatusAsync(dto, currentUserId);

                if (task == null)
                    return NotFound(new { message = "Görev bulunamadı." });

                return Ok(task);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Görev durumu güncellenirken bir hata oluştu.", detail = ex.Message });
            }
        }

        /// <summary>Görev ataması değiştir (Sadece Manager'lar)</summary>
        [HttpPut("{id}/assign")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AssignWorkTask(Guid id, [FromBody] WorkTaskAssignmentUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                dto.WorkTaskId = id;
                var currentUserId = GetCurrentUserId();
                var task = await _workTaskService.AssignWorkTaskAsync(dto, currentUserId);

                if (task == null)
                    return NotFound(new { message = "Görev bulunamadı." });

                return Ok(task);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Görev ataması değiştirilirken bir hata oluştu.", detail = ex.Message });
            }
        }

        // WorkTask Queries

        /// <summary>Filtrelenmiş görev listesi</summary>
        [HttpPost("filter")]
        public async Task<IActionResult> GetFilteredWorkTasks([FromBody] WorkTaskFilterDto filter)
        {   
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                // SalesPerson sadece kendi task'larını görebilir
                if (currentUserRole == UserRole.SalesPerson)
                {
                    filter.AssignedToUserId = currentUserId;
                }

                var result = await _workTaskService.GetFilteredWorkTasksAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Görevler alınırken bir hata oluştu.", detail = ex.Message });
            }
        }

        /// <summary>Bana atanan görevler</summary> 
        [HttpGet("my-tasks")]
        public async Task<IActionResult> GetMyWorkTasks()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var tasks = await _workTaskService.GetWorkTasksByAssigneeAsync(currentUserId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Görevler alınırken bir hata oluştu.", detail = ex.Message });
            }
        }

        /// <summary>Oluşturduğum görevler (Manager'lar için)</summary>
        [HttpGet("created-by-me")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetCreatedWorkTasks()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var tasks = await _workTaskService.GetWorkTasksByCreatorAsync(currentUserId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Görevler alınırken bir hata oluştu.", detail = ex.Message });
            }
        }

        /// <summary>Duruma göre görevler</summary>
        [HttpGet("by-status/{status}")]
        public async System.Threading.Tasks.Task<IActionResult> GetWorkTasksByStatus(FuarYonetimSistemi.Domain.Enums.WorkTaskStatus status)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();
                var tasks = await _workTaskService.GetWorkTasksByStatusAsync(status);

                // SalesPerson sadece kendi task'larını görebilir
                if (currentUserRole == UserRole.SalesPerson)
                {
                    tasks = tasks.Where(t => t.AssignedToUserId == currentUserId);
                }

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Görevler alınırken bir hata oluştu.", detail = ex.Message });
            }
        }

        /// <summary>Süresi geçmiş görevler</summary>
        [HttpGet("overdue")]
        public async System.Threading.Tasks.Task<IActionResult> GetOverdueWorkTasks()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();
                var tasks = await _workTaskService.GetOverdueWorkTasksAsync();

                // SalesPerson sadece kendi task'larını görebilir
                if (currentUserRole == UserRole.SalesPerson)
                {
                    tasks = tasks.Where(t => t.AssignedToUserId == currentUserId);
                }

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Süresi geçmiş görevler alınırken bir hata oluştu.", detail = ex.Message });
            }
        }

        /// <summary>Yakında süresi dolacak görevler</summary>
        [HttpGet("due-soon")]
        public async Task<IActionResult> GetWorkTasksDueSoon([FromQuery] int daysAhead = 3)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();
                var tasks = await _workTaskService.GetWorkTasksDueSoonAsync(daysAhead);

                // SalesPerson sadece kendi task'larını görebilir
                if (currentUserRole == UserRole.SalesPerson)
                {
                    tasks = tasks.Where(t => t.AssignedToUserId == currentUserId);
                }

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Yakında süresi dolacak görevler alınırken bir hata oluştu.", detail = ex.Message });
            }
        }

        // WorkTask Comments

        /// <summary>Göreve yorum ekle</summary>
        [HttpPost("{id}/comments")]
        public async Task<IActionResult> AddWorkTaskComment(Guid id, [FromBody] WorkTaskCommentCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                dto.WorkTaskId = id;
                var currentUserId = GetCurrentUserId();
                var comment = await _workTaskService.AddWorkTaskCommentAsync(dto, currentUserId);
                return CreatedAtAction(nameof(GetWorkTaskComments), new { id }, comment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Yorum eklenirken bir hata oluştu.", detail = ex.Message });
            }
        }

        /// <summary>Görev yorumlarını getir</summary>
        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetWorkTaskComments(Guid id)
        {
            try
            {
                var comments = await _workTaskService.GetWorkTaskCommentsAsync(id);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Yorumlar alınırken bir hata oluştu.", detail = ex.Message });
            }
        }

        /// <summary>Yorum sil</summary>
        [HttpDelete("comments/{commentId}")]
        public async Task<IActionResult> DeleteWorkTaskComment(Guid commentId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var result = await _workTaskService.DeleteWorkTaskCommentAsync(commentId, currentUserId);

                if (!result)
                    return NotFound(new { message = "Yorum bulunamadı." });

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Yorum silinirken bir hata oluştu.", detail = ex.Message });
            }
        }

        // WorkTask History

        /// <summary>Görev geçmişi</summary>
        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetWorkTaskHistory(Guid id)
        {
            try
            {
                var history = await _workTaskService.GetWorkTaskHistoryAsync(id);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Görev geçmişi alınırken bir hata oluştu.", detail = ex.Message });
            }
        }

        // Dashboard & Statistics

        /// <summary>Görev dashboard'u</summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetWorkTaskDashboard()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                // SalesPerson sadece kendi dashboard'unu görebilir
                var userId = currentUserRole == UserRole.SalesPerson ? currentUserId : (Guid?)null;

                var dashboard = await _workTaskService.GetWorkTaskDashboardAsync(userId);
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Dashboard alınırken bir hata oluştu.", detail = ex.Message });
            }
        }

        /// <summary>Görev istatistikleri</summary>
        [HttpGet("statistics")]
        public async  Task<IActionResult> GetWorkTaskStatistics()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                // SalesPerson sadece kendi istatistiklerini görebilir
                var userId = currentUserRole == UserRole.SalesPerson ? currentUserId : (Guid?)null;

                var statistics = await _workTaskService.GetWorkTaskStatisticsAsync(userId);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "İstatistikler alınırken bir hata oluştu.", detail = ex.Message });
            }
        }

        // Reporting

        /// <summary>Görevleri Excel'e aktar</summary>
        [HttpPost("export-excel")]
        public async Task<IActionResult> ExportWorkTasksToExcel([FromBody] WorkTaskFilterDto filter)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                // SalesPerson sadece kendi task'larını export edebilir
                if (currentUserRole == UserRole.SalesPerson)
                {
                    filter.AssignedToUserId = currentUserId;
                }

                var excelBytes = await _workTaskService.ExportWorkTasksToExcelAsync(filter);
                var fileName = $"Gorevler_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Excel dosyası oluşturulurken bir hata oluştu.", detail = ex.Message });
            }
        }

        // Quick Actions

        /// <summary>Görevi tamamla</summary>
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteWorkTask(Guid id, [FromBody] string? completionNote = null)
        {
            try
            {
                var dto = new WorkTaskStatusUpdateDto
                {
                    WorkTaskId = id,
                    Status =WorkTaskStatus.Completed,
                    StatusChangeComment = completionNote ?? "Görev tamamlandı."
                };

                var currentUserId = GetCurrentUserId();
                var task = await _workTaskService.UpdateWorkTaskStatusAsync(dto, currentUserId);

                if (task == null)
                    return NotFound(new { message = "Görev bulunamadı." });

                return Ok(task);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Görev tamamlanırken bir hata oluştu.", detail = ex.Message });
            }
        }

        /// <summary>Görevi başlat</summary>
        [HttpPost("{id}/start")]
        public async Task<IActionResult> StartWorkTask(Guid id, [FromBody] string? startNote = null)
        {
            try
            {
                var dto = new WorkTaskStatusUpdateDto
                {
                    WorkTaskId = id,
                    Status = WorkTaskStatus.InProgress,  // Type name ile erişim
                    StatusChangeComment = startNote ?? "Görev başlatıldı."
                };

                var currentUserId = GetCurrentUserId();
                var task = await _workTaskService.UpdateWorkTaskStatusAsync(dto, currentUserId);

                if (task == null)
                    return NotFound(new { message = "Görev bulunamadı." });

                return Ok(task);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Görev başlatılırken bir hata oluştu.", detail = ex.Message });
            }
        }

        /// <summary>Görevi iptal et (Sadece Manager'lar)</summary>
        [HttpPost("{id}/cancel")]
        [Authorize(Roles = "Manager")]
        public async System.Threading.Tasks.Task<IActionResult> CancelWorkTask(Guid id, [FromBody] string? cancellationReason = null)
        {
            try
            {
                var dto = new WorkTaskStatusUpdateDto
                {
                    WorkTaskId = id,
                    Status = WorkTaskStatus.Cancelled,
                    StatusChangeComment = cancellationReason ?? "Görev iptal edildi."
                };

                var currentUserId = GetCurrentUserId();
                var task = await _workTaskService.UpdateWorkTaskStatusAsync(dto, currentUserId);

                if (task == null)
                    return NotFound(new { message = "Görev bulunamadı." });

                return Ok(task);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Görev iptal edilirken bir hata oluştu.", detail = ex.Message });
            }
        }

        /// <summary>Toplu görev durumu değiştir (Sadece Manager'lar)</summary>
        [HttpPost("bulk-status-update")]
        [Authorize(Roles = "Manager")]
        public async System.Threading.Tasks.Task<IActionResult> BulkUpdateWorkTaskStatus([FromBody] BulkWorkTaskStatusUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var currentUserId = GetCurrentUserId();
                var results = new List<object>();

                foreach (var workTaskId in dto.WorkTaskIds)
                {
                    try
                    {
                        var updateDto = new WorkTaskStatusUpdateDto
                        {
                            WorkTaskId = workTaskId,
                            Status = dto.Status,
                            StatusChangeComment = dto.Comment
                        };

                        var task = await _workTaskService.UpdateWorkTaskStatusAsync(updateDto, currentUserId);
                        results.Add(new { WorkTaskId = workTaskId, Success = true, Task = task });
                    }
                    catch (Exception ex)
                    {
                        results.Add(new { WorkTaskId = workTaskId, Success = false, Error = ex.Message });
                    }
                }

                return Ok(new { Results = results });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Toplu güncelleme yapılırken bir hata oluştu.", detail = ex.Message });
            }
        }
    }

    // Additional DTOs for bulk operations
    public class BulkWorkTaskStatusUpdateDto
    {
        [Required]
        public List<Guid> WorkTaskIds { get; set; } = new List<Guid>();

        [Required]
        public FuarYonetimSistemi.Domain.Enums.WorkTaskStatus Status { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }
}