using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Domain.Entities;
using FuarYonetimSistemi.Domain.Enums;
using System;
using System.Collections.Generic;
using WorkTaskStatus = FuarYonetimSistemi.Domain.Enums.WorkTaskStatus;

namespace FuarYonetimSistemi.Application.Interfaces
{
    public interface IWorkTaskService
    {
        // Task CRUD Operations
       Task<WorkTaskDto> CreateWorkTaskAsync(WorkTaskCreateDto dto, Guid createdByUserId);
       Task<WorkTaskDto?> GetWorkTaskByIdAsync(Guid workTaskId);
       Task<WorkTaskDto?> UpdateWorkTaskAsync(WorkTaskUpdateDto dto, Guid updatedByUserId);
       Task<bool> DeleteWorkTaskAsync(Guid workTaskId, Guid deletedByUserId);

        // Task Status Management
       Task<WorkTaskDto?> UpdateWorkTaskStatusAsync(WorkTaskStatusUpdateDto dto, Guid updatedByUserId);
       Task<WorkTaskDto?> AssignWorkTaskAsync(WorkTaskAssignmentUpdateDto dto, Guid updatedByUserId);

        // Task Queries
       Task<PagedResult<WorkTaskDto>> GetFilteredWorkTasksAsync(WorkTaskFilterDto filter);
       Task<IEnumerable<WorkTaskDto>> GetWorkTasksByAssigneeAsync(Guid assigneeUserId);
       Task<IEnumerable<WorkTaskDto>> GetWorkTasksByCreatorAsync(Guid creatorUserId);
       Task<IEnumerable<WorkTaskDto>> GetWorkTasksByStatusAsync(WorkTaskStatus status);
       Task<IEnumerable<WorkTaskDto>> GetOverdueWorkTasksAsync();
       Task<IEnumerable<WorkTaskDto>> GetWorkTasksDueSoonAsync(int daysAhead = 3);

        // Task Comments
       Task<WorkTaskCommentDto> AddWorkTaskCommentAsync(WorkTaskCommentCreateDto dto, Guid createdByUserId);
       Task<IEnumerable<WorkTaskCommentDto>> GetWorkTaskCommentsAsync(Guid workTaskId);
       Task<bool> DeleteWorkTaskCommentAsync(Guid commentId, Guid deletedByUserId);

        // Task History
       Task<IEnumerable<WorkTaskHistoryDto>> GetWorkTaskHistoryAsync(Guid workTaskId);

        // Dashboard & Statistics
       Task<WorkTaskDashboardDto> GetWorkTaskDashboardAsync(Guid? userId = null);
       Task<WorkTaskStatisticsDto> GetWorkTaskStatisticsAsync(Guid? userId = null);

        // Reporting
       Task<byte[]> ExportWorkTasksToExcelAsync(WorkTaskFilterDto filter);
    }

    // Additional DTOs for Dashboard and Statistics
    public class WorkTaskDashboardDto
    {
        public int TotalTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int TasksDueSoon { get; set; }
        public List<WorkTaskDto> RecentTasks { get; set; } = new List<WorkTaskDto>();
        public List<WorkTaskDto> HighPriorityTasks { get; set; } = new List<WorkTaskDto>();
    }

    public class WorkTaskStatisticsDto
    {
        public Dictionary<WorkTaskStatus, int> TasksByStatus { get; set; } = new Dictionary<WorkTaskStatus, int>();
        public Dictionary<TaskPriority, int> TasksByPriority { get; set; } = new Dictionary<TaskPriority, int>();
        public Dictionary<string, int> TasksByAssignee { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> TasksByCreator { get; set; } = new Dictionary<string, int>();
        public double AverageCompletionTimeInDays { get; set; }
        public int TasksCompletedThisWeek { get; set; }
        public int TasksCompletedThisMonth { get; set; }
    }
}