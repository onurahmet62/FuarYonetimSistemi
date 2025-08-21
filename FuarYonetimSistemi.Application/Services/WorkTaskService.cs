using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Domain.Entities;
using FuarYonetimSistemi.Domain.Enums;
using FuarYonetimSistemi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkTaskStatus = FuarYonetimSistemi.Domain.Enums.WorkTaskStatus;

namespace FuarYonetimSistemi.Application.Services
{
    public class WorkTaskService : IWorkTaskService
    {
        private readonly AppDbContext _context;

        public WorkTaskService(AppDbContext context)
        {
            _context = context;
        }

        public async  Task<WorkTaskDto> CreateWorkTaskAsync(WorkTaskCreateDto dto, Guid createdByUserId)
        {
            // Validation
            var assignee = await _context.Users.FindAsync(dto.AssignedToUserId);
            if (assignee == null || assignee.IsDeleted)
                throw new ArgumentException("Atanan kullanıcı bulunamadı.");

            var creator = await _context.Users.FindAsync(createdByUserId);
            if (creator == null || creator.Role != UserRole.Manager)
                throw new UnauthorizedAccessException("Sadece Manager'lar görev oluşturabilir.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var workTask = new WorkTask
                {
                    Id = Guid.NewGuid(),
                    Title = dto.Title,
                    Description = dto.Description,
                    Priority = dto.Priority,
                    DueDate = dto.DueDate,
                    Notes = dto.Notes,
                    CreatedByUserId = createdByUserId,
                    AssignedToUserId = dto.AssignedToUserId,
                    Status = WorkTaskStatus.Planned,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.WorkTasks.Add(workTask);
                await _context.SaveChangesAsync();

                // Task History kaydı
                await AddWorkTaskHistoryAsync(workTask.Id, "Task Created", null, "Task created and assigned", createdByUserId);

                await transaction.CommitAsync();
                return await GetWorkTaskByIdAsync(workTask.Id) ?? throw new InvalidOperationException("Task could not be retrieved after creation.");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async System.Threading.Tasks.Task<WorkTaskDto?> GetWorkTaskByIdAsync(Guid workTaskId)
        {
            var workTask = await _context.WorkTasks
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .Include(t => t.WorkTaskHistories)
                    .ThenInclude(h => h.ChangedByUser)
                .Include(t => t.WorkTaskComments.Where(c => !c.IsDeleted))
                    .ThenInclude(c => c.CreatedByUser)
                .FirstOrDefaultAsync(t => t.Id == workTaskId && !t.IsDeleted);

            if (workTask == null) return null;

            return MapToWorkTaskDto(workTask);
        }

        public async System.Threading.Tasks.Task<WorkTaskDto?> UpdateWorkTaskAsync(WorkTaskUpdateDto dto, Guid updatedByUserId)
        {
            var workTask = await _context.WorkTasks.FindAsync(dto.Id);
            if (workTask == null || workTask.IsDeleted)
                return null;

            var updater = await _context.Users.FindAsync(updatedByUserId);
            if (updater == null)
                throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");

            // Yetki kontrolü: Manager her şeyi güncelleyebilir, SalesPerson sadece kendi task'larının durumunu güncelleyebilir
            if (updater.Role != UserRole.Manager && workTask.AssignedToUserId != updatedByUserId)
                throw new UnauthorizedAccessException("Bu görev üzerinde değişiklik yapma yetkiniz yok.");

            var changes = new List<(string action, string? oldValue, string? newValue)>();

            if (!string.IsNullOrEmpty(dto.Title) && dto.Title != workTask.Title)
            {
                changes.Add(("Title Changed", workTask.Title, dto.Title));
                workTask.Title = dto.Title;
            }

            if (dto.Description != null && dto.Description != workTask.Description)
            {
                changes.Add(("Description Changed", workTask.Description, dto.Description));
                workTask.Description = dto.Description;
            }

            if (dto.Status.HasValue && dto.Status != workTask.Status)
            {
                changes.Add(("Status Changed", workTask.Status.ToString(), dto.Status.ToString()));
                workTask.Status = dto.Status.Value;

                if (dto.Status == WorkTaskStatus.Completed)
                    workTask.CompletedAt = DateTime.UtcNow;
                else if (workTask.CompletedAt.HasValue)
                    workTask.CompletedAt = null;
            }

            if (dto.Priority.HasValue && dto.Priority != workTask.Priority)
            {
                changes.Add(("Priority Changed", workTask.Priority.ToString(), dto.Priority.ToString()));
                workTask.Priority = dto.Priority.Value;
            }

            if (dto.DueDate != workTask.DueDate)
            {
                changes.Add(("Due Date Changed", workTask.DueDate?.ToString(), dto.DueDate?.ToString()));
                workTask.DueDate = dto.DueDate;
            }

            if (dto.AssignedToUserId.HasValue && dto.AssignedToUserId != workTask.AssignedToUserId)
            {
                // Sadece Manager assignment değiştirebilir
                if (updater.Role != UserRole.Manager)
                    throw new UnauthorizedAccessException("Sadece Manager'lar görev ataması değiştirebilir.");

                var newAssignee = await _context.Users.FindAsync(dto.AssignedToUserId.Value);
                if (newAssignee == null || newAssignee.IsDeleted)
                    throw new ArgumentException("Yeni atanan kullanıcı bulunamadı.");

                var oldAssignee = await _context.Users.FindAsync(workTask.AssignedToUserId);
                changes.Add(("Assignment Changed", oldAssignee?.FullName, newAssignee.FullName));
                workTask.AssignedToUserId = dto.AssignedToUserId.Value;
            }

            if (dto.Notes != workTask.Notes)
            {
                changes.Add(("Notes Updated", workTask.Notes, dto.Notes));
                workTask.Notes = dto.Notes;
            }

            workTask.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // History kayıtlarını ekle
            foreach (var (action, oldValue, newValue) in changes)
            {
                await AddWorkTaskHistoryAsync(workTask.Id, action, oldValue, newValue, updatedByUserId);
            }

            return await GetWorkTaskByIdAsync(workTask.Id);
        }

        public async System.Threading.Tasks.Task<WorkTaskDto?> UpdateWorkTaskStatusAsync(WorkTaskStatusUpdateDto dto, Guid updatedByUserId)
        {
            var updateDto = new WorkTaskUpdateDto
            {
                Id = dto.WorkTaskId,
                Status = (WorkTaskStatus?)dto.Status
            };

            var result = await UpdateWorkTaskAsync(updateDto, updatedByUserId);

            // Status değişikliği için özel comment ekle
            if (!string.IsNullOrEmpty(dto.StatusChangeComment))
            {
                await AddWorkTaskCommentAsync(new WorkTaskCommentCreateDto
                {
                    WorkTaskId = dto.WorkTaskId,
                    Comment = $"Status değiştirildi: {dto.StatusChangeComment}"
                }, updatedByUserId);
            }

            return result;
        }

        public async System.Threading.Tasks.Task<WorkTaskDto?> AssignWorkTaskAsync(WorkTaskAssignmentUpdateDto dto, Guid updatedByUserId)
        {
            var updateDto = new WorkTaskUpdateDto
            {
                Id = dto.WorkTaskId,
                AssignedToUserId = dto.NewAssignedToUserId
            };

            var result = await UpdateWorkTaskAsync(updateDto, updatedByUserId);

            // Assignment değişikliği için özel comment ekle
            if (!string.IsNullOrEmpty(dto.AssignmentComment))
            {
                await AddWorkTaskCommentAsync(new WorkTaskCommentCreateDto
                {
                    WorkTaskId = dto.WorkTaskId,
                    Comment = $"Görev yeniden atandı: {dto.AssignmentComment}"
                }, updatedByUserId);
            }

            return result;
        }

        public async System.Threading.Tasks.Task<bool> DeleteWorkTaskAsync(Guid workTaskId, Guid deletedByUserId)
        {
            var workTask = await _context.WorkTasks.FindAsync(workTaskId);
            if (workTask == null || workTask.IsDeleted)
                return false;

            var deleter = await _context.Users.FindAsync(deletedByUserId);
            if (deleter?.Role != UserRole.Manager)
                throw new UnauthorizedAccessException("Sadece Manager'lar görev silebilir.");

            workTask.IsDeleted = true;
            workTask.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await AddWorkTaskHistoryAsync(workTaskId, "Task Deleted", null, "Task marked as deleted", deletedByUserId);

            return true;
        }

        public async System.Threading.Tasks.Task<PagedResult<WorkTaskDto>> GetFilteredWorkTasksAsync(WorkTaskFilterDto filter)
        {
            var query = _context.WorkTasks
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .Where(t => !t.IsDeleted)
                .AsQueryable();

            // Filtering
            if (filter.Status.HasValue)
                query = query.Where(t => t.Status == filter.Status.Value);

            if (filter.Priority.HasValue)
                query = query.Where(t => t.Priority == filter.Priority.Value);

            if (filter.AssignedToUserId.HasValue)
                query = query.Where(t => t.AssignedToUserId == filter.AssignedToUserId.Value);

            if (filter.CreatedByUserId.HasValue)
                query = query.Where(t => t.CreatedByUserId == filter.CreatedByUserId.Value);

            if (filter.DueDateFrom.HasValue)
                query = query.Where(t => t.DueDate >= filter.DueDateFrom.Value);

            if (filter.DueDateTo.HasValue)
                query = query.Where(t => t.DueDate <= filter.DueDateTo.Value);

            if (filter.CreatedFrom.HasValue)
                query = query.Where(t => t.CreatedAt >= filter.CreatedFrom.Value);

            if (filter.CreatedTo.HasValue)
                query = query.Where(t => t.CreatedAt <= filter.CreatedTo.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(t =>
                    t.Title.Contains(filter.SearchTerm) ||
                    (t.Description != null && t.Description.Contains(filter.SearchTerm)) ||
                    (t.Notes != null && t.Notes.Contains(filter.SearchTerm)));
            }

            var totalCount = await query.CountAsync();

            // Sorting
            query = filter.SortBy.ToLower() switch
            {
                "title" => filter.IsDescending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                "status" => filter.IsDescending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
                "priority" => filter.IsDescending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
                "duedate" => filter.IsDescending ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),
                "updatedat" => filter.IsDescending ? query.OrderByDescending(t => t.UpdatedAt) : query.OrderBy(t => t.UpdatedAt),
                _ => filter.IsDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt)
            };

            var tasks = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<WorkTaskDto>
            {
                Items = tasks.Select(MapToWorkTaskDto),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async System.Threading.Tasks.Task<IEnumerable<WorkTaskDto>> GetWorkTasksByAssigneeAsync(Guid assigneeUserId)
        {
            var tasks = await _context.WorkTasks
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .Where(t => t.AssignedToUserId == assigneeUserId && !t.IsDeleted)
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync();

            return tasks.Select(MapToWorkTaskDto);
        }

        public async System.Threading.Tasks.Task<IEnumerable<WorkTaskDto>> GetWorkTasksByCreatorAsync(Guid creatorUserId)
        {
            var tasks = await _context.WorkTasks
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .Where(t => t.CreatedByUserId == creatorUserId && !t.IsDeleted)
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync();

            return tasks.Select(MapToWorkTaskDto);
        }

        public async Task<IEnumerable<WorkTaskDto>> GetWorkTasksByStatusAsync(WorkTaskStatus status)
        {
            var tasks = await _context.WorkTasks
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .Where(t => t.Status == status && !t.IsDeleted)
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync();

            return tasks.Select(MapToWorkTaskDto);
        }

        public async System.Threading.Tasks.Task<IEnumerable<WorkTaskDto>> GetOverdueWorkTasksAsync()
        {
            var today = DateTime.UtcNow.Date;
            var tasks = await _context.WorkTasks
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .Where(t => t.DueDate.HasValue &&
                           t.DueDate.Value.Date < today &&
                           t.Status != WorkTaskStatus.Completed &&
                           t.Status != WorkTaskStatus.Cancelled &&
                           !t.IsDeleted)
                .OrderBy(t => t.DueDate)
                .ToListAsync();

            return tasks.Select(MapToWorkTaskDto);
        }

        public async System.Threading.Tasks.Task<IEnumerable<WorkTaskDto>> GetWorkTasksDueSoonAsync(int daysAhead = 3)
        {
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(daysAhead);

            var tasks = await _context.WorkTasks
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .Where(t => t.DueDate.HasValue &&
                           t.DueDate.Value >= startDate &&
                           t.DueDate.Value <= endDate &&
                           t.Status != WorkTaskStatus.Completed &&
                           t.Status != WorkTaskStatus.Cancelled &&
                           !t.IsDeleted)
                .OrderBy(t => t.DueDate)
                .ToListAsync();

            return tasks.Select(MapToWorkTaskDto);
        }

        // Comments, History, Dashboard methods continue...
        public async System.Threading.Tasks.Task<WorkTaskCommentDto> AddWorkTaskCommentAsync(WorkTaskCommentCreateDto dto, Guid createdByUserId)
        {
            var workTask = await _context.WorkTasks.FindAsync(dto.WorkTaskId);
            if (workTask == null || workTask.IsDeleted)
                throw new ArgumentException("Görev bulunamadı.");

            var comment = new WorkTaskComment
            {
                Id = Guid.NewGuid(),
                WorkTaskId = dto.WorkTaskId,
                Comment = dto.Comment,
                CreatedByUserId = createdByUserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.WorkTaskComments.Add(comment);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(createdByUserId);
            return new WorkTaskCommentDto
            {
                Id = comment.Id,
                Comment = comment.Comment,
                CreatedAt = comment.CreatedAt,
                CreatedByUserId = comment.CreatedByUserId,
                CreatedByUserName = user?.FullName ?? "Unknown"
            };
        }

        public async System.Threading.Tasks.Task<IEnumerable<WorkTaskCommentDto>> GetWorkTaskCommentsAsync(Guid workTaskId)
        {
            var comments = await _context.WorkTaskComments
                .Include(c => c.CreatedByUser)
                .Where(c => c.WorkTaskId == workTaskId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return comments.Select(c => new WorkTaskCommentDto
            {
                Id = c.Id,
                Comment = c.Comment,
                CreatedAt = c.CreatedAt,
                CreatedByUserId = c.CreatedByUserId,
                CreatedByUserName = c.CreatedByUser.FullName
            });
        }

        public async System.Threading.Tasks.Task<bool> DeleteWorkTaskCommentAsync(Guid commentId, Guid deletedByUserId)
        {
            var comment = await _context.WorkTaskComments.FindAsync(commentId);
            if (comment == null || comment.IsDeleted)
                return false;

            var deleter = await _context.Users.FindAsync(deletedByUserId);
            if (comment.CreatedByUserId != deletedByUserId && deleter?.Role != UserRole.Manager)
                throw new UnauthorizedAccessException("Bu yorumu silme yetkiniz yok.");

            comment.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async System.Threading.Tasks.Task<IEnumerable<WorkTaskHistoryDto>> GetWorkTaskHistoryAsync(Guid workTaskId)
        {
            var histories = await _context.WorkTaskHistories
                .Include(h => h.ChangedByUser)
                .Where(h => h.WorkTaskId == workTaskId)
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();

            return histories.Select(h => new WorkTaskHistoryDto
            {
                Id = h.Id,
                Action = h.Action,
                OldValue = h.OldValue,
                NewValue = h.NewValue,
                Description = h.Description,
                ChangedAt = h.ChangedAt,
                ChangedByUserId = h.ChangedByUserId,
                ChangedByUserName = h.ChangedByUser.FullName
            });
        }

        public async System.Threading.Tasks.Task<WorkTaskDashboardDto> GetWorkTaskDashboardAsync(Guid? userId = null)
        {
            var query = _context.WorkTasks.Where(t => !t.IsDeleted);

            if (userId.HasValue)
            {
                var user = await _context.Users.FindAsync(userId.Value);
                if (user?.Role == UserRole.SalesPerson)
                    query = query.Where(t => t.AssignedToUserId == userId.Value);
                else if (user?.Role == UserRole.Manager)
                    query = query.Where(t => t.CreatedByUserId == userId.Value);
            }

            var tasks = await query.Include(t => t.CreatedByUser).Include(t => t.AssignedToUser).ToListAsync();
            var now = DateTime.UtcNow;

            return new WorkTaskDashboardDto
            {
                TotalTasks = tasks.Count,
                PendingTasks = tasks.Count(t => t.Status == WorkTaskStatus.Planned),
                InProgressTasks = tasks.Count(t => t.Status == WorkTaskStatus.InProgress),
                CompletedTasks = tasks.Count(t => t.Status == WorkTaskStatus.Completed),
                OverdueTasks = tasks.Count(t => t.DueDate.HasValue && t.DueDate.Value < now && t.Status != WorkTaskStatus.Completed && t.Status != WorkTaskStatus.Cancelled),
                TasksDueSoon = tasks.Count(t => t.DueDate.HasValue && t.DueDate.Value >= now && t.DueDate.Value <= now.AddDays(3)),
                RecentTasks = tasks.OrderByDescending(t => t.UpdatedAt).Take(5).Select(MapToWorkTaskDto).ToList(),
                HighPriorityTasks = tasks.Where(t => t.Priority >= TaskPriority.High && t.Status != WorkTaskStatus.Completed).Select(MapToWorkTaskDto).ToList()
            };
        }

        public async System.Threading.Tasks.Task<WorkTaskStatisticsDto> GetWorkTaskStatisticsAsync(Guid? userId = null)
        {
            var query = _context.WorkTasks.Where(t => !t.IsDeleted);

            if (userId.HasValue)
            {
                var user = await _context.Users.FindAsync(userId.Value);
                if (user?.Role == UserRole.SalesPerson)
                    query = query.Where(t => t.AssignedToUserId == userId.Value);
                else if (user?.Role == UserRole.Manager)
                    query = query.Where(t => t.CreatedByUserId == userId.Value);
            }

            var tasks = await query
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .ToListAsync();

            var completedTasks = tasks.Where(t => t.Status == WorkTaskStatus.Completed && t.CompletedAt.HasValue).ToList();
            var now = DateTime.UtcNow;
            var weekStart = now.AddDays(-(int)now.DayOfWeek);
            var monthStart = new DateTime(now.Year, now.Month, 1);

            return new WorkTaskStatisticsDto
            {
                TasksByStatus = Enum.GetValues<WorkTaskStatus>().ToDictionary(s => s, s => tasks.Count(t => t.Status == s)),
                TasksByPriority = Enum.GetValues<TaskPriority>().ToDictionary(p => p, p => tasks.Count(t => t.Priority == p)),
                TasksByAssignee = tasks.GroupBy(t => t.AssignedToUser.FullName).ToDictionary(g => g.Key, g => g.Count()),
                TasksByCreator = tasks.GroupBy(t => t.CreatedByUser.FullName).ToDictionary(g => g.Key, g => g.Count()),
                AverageCompletionTimeInDays = completedTasks.Any() ?
                    completedTasks.Average(t => (t.CompletedAt!.Value - t.CreatedAt).TotalDays) : 0,
                TasksCompletedThisWeek = completedTasks.Count(t => t.CompletedAt >= weekStart),
                TasksCompletedThisMonth = completedTasks.Count(t => t.CompletedAt >= monthStart)
            };
        }

        public async System.Threading.Tasks.Task<byte[]> ExportWorkTasksToExcelAsync(WorkTaskFilterDto filter)
        {
            filter.PageSize = int.MaxValue;
            var result = await GetFilteredWorkTasksAsync(filter);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Görevler");

            var headers = new[] {
                "Başlık", "Açıklama", "Durum", "Öncelik", "Oluşturan", "Atanan",
                "Oluşturma Tarihi", "Güncelleme Tarihi", "Bitiş Tarihi", "Tamamlanma Tarihi", "Notlar"
            };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cells[1, i + 1].Value = headers[i];

            sheet.Row(1).Style.Font.Bold = true;

            int row = 2;
            foreach (var task in result.Items)
            {
                sheet.Cells[row, 1].Value = task.Title;
                sheet.Cells[row, 2].Value = task.Description;
                sheet.Cells[row, 3].Value = GetStatusDisplayName(task.Status);
                sheet.Cells[row, 4].Value = GetPriorityDisplayName(task.Priority);
                sheet.Cells[row, 5].Value = task.CreatedByUserName;
                sheet.Cells[row, 6].Value = task.AssignedToUserName;
                sheet.Cells[row, 7].Value = task.CreatedAt.ToString("dd.MM.yyyy HH:mm");
                sheet.Cells[row, 8].Value = task.UpdatedAt.ToString("dd.MM.yyyy HH:mm");
                sheet.Cells[row, 9].Value = task.DueDate?.ToString("dd.MM.yyyy") ?? "";
                sheet.Cells[row, 10].Value = task.CompletedAt?.ToString("dd.MM.yyyy HH:mm") ?? "";
                sheet.Cells[row, 11].Value = task.Notes;
                row++;
            }

            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            return package.GetAsByteArray();
        }

        // Helper methods
        private async System.Threading.Tasks.Task AddWorkTaskHistoryAsync(Guid workTaskId, string action, string? oldValue, string? newValue, Guid changedByUserId)
        {
            var history = new WorkTaskHistory
            {
                Id = Guid.NewGuid(),
                WorkTaskId = workTaskId,
                Action = action,
                OldValue = oldValue,
                NewValue = newValue,
                Description = $"{action}: {oldValue} → {newValue}",
                ChangedByUserId = changedByUserId,
                ChangedAt = DateTime.UtcNow
            };

            _context.WorkTaskHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        private WorkTaskDto MapToWorkTaskDto(WorkTask workTask)
        {
            return new WorkTaskDto
            {
                Id = workTask.Id,
                Title = workTask.Title,
                Description = workTask.Description,
                Status = workTask.Status,
                Priority = workTask.Priority,
                CreatedAt = workTask.CreatedAt,
                UpdatedAt = workTask.UpdatedAt,
                DueDate = workTask.DueDate,
                CompletedAt = workTask.CompletedAt,
                Notes = workTask.Notes,
                CreatedByUserId = workTask.CreatedByUserId,
                CreatedByUserName = workTask.CreatedByUser?.FullName ?? "Unknown",
                AssignedToUserId = workTask.AssignedToUserId,
                AssignedToUserName = workTask.AssignedToUser?.FullName ?? "Unknown",
                WorkTaskHistories = workTask.WorkTaskHistories?.Select(h => new WorkTaskHistoryDto
                {
                    Id = h.Id,
                    Action = h.Action,
                    OldValue = h.OldValue,
                    NewValue = h.NewValue,
                    Description = h.Description,
                    ChangedAt = h.ChangedAt,
                    ChangedByUserId = h.ChangedByUserId,
                    ChangedByUserName = h.ChangedByUser?.FullName ?? "Unknown"
                }).ToList(),
                WorkTaskComments = workTask.WorkTaskComments?.Where(c => !c.IsDeleted).Select(c => new WorkTaskCommentDto
                {
                    Id = c.Id,
                    Comment = c.Comment,
                    CreatedAt = c.CreatedAt,
                    CreatedByUserId = c.CreatedByUserId,
                    CreatedByUserName = c.CreatedByUser?.FullName ?? "Unknown"
                }).ToList()
            };
        }

        private static string GetStatusDisplayName(WorkTaskStatus status) => status switch
        {
            WorkTaskStatus.Planned => "Planlandı",
            WorkTaskStatus.InProgress => "Yapılıyor",
            WorkTaskStatus.Completed => "Yapıldı",
            WorkTaskStatus.Cancelled => "İptal Edildi",
            _ => status.ToString()
        };

        private static string GetPriorityDisplayName(TaskPriority priority) => priority switch
        {
            TaskPriority.Low => "Düşük",
            TaskPriority.Medium => "Orta",
            TaskPriority.High => "Yüksek",
            TaskPriority.Critical => "Kritik",
            _ => priority.ToString()
        };

      
    }
}