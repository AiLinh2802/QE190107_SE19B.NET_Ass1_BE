using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories;
using TaskTrack.Service.Contracts;
using TaskTrack.Service.Exceptions;
using TaskTrack.Service.Interfaces;
using TaskTrack.Service.Mapping;

namespace TaskTrack.Service.Services;

public class TaskService(ITaskRepository repository) : ITaskService
{
    public async Task<IReadOnlyList<TaskDto>> GetAllAsync() =>
        (await repository.GetActiveAsync()).Select(x => x.ToDto()).ToList();

    public async Task<IReadOnlyList<TaskDto>> GetByProjectAsync(int projectId) =>
        (await repository.GetByProjectAsync(projectId)).Select(x => x.ToDto()).ToList();

    public async Task<IReadOnlyList<TaskDto>> SearchAsync(string? title, short? status, short? priority, int? projectId, int? tagId)
    {
        ValidateRange(status, "status");
        ValidateRange(priority, "priority");
        return (await repository.SearchAsync(title, status, priority, projectId, tagId)).Select(x => x.ToDto()).ToList();
    }

    public async Task<TaskDto> GetByIdAsync(int id)
    {
        var task = await repository.GetDetailsAsync(id) ?? throw ServiceException.NotFound("Task", id);
        return task.ToDto();
    }

    public async Task<TaskDto> CreateAsync(TaskRequest request)
    {
        var tagIds = await ValidateReferencesAsync(request);
        var task = new TaskItem
        {
            Title = request.Title.Trim(),
            Description = Clean(request.Description),
            Status = request.Status,
            Priority = request.Priority,
            DueDate = request.DueDate,
            ProjectId = request.ProjectId,
            IsActive = request.IsActive,
            CreatedDate = Now(),
            TaskTags = tagIds.Select(tagId => new TaskTag { TagId = tagId }).ToList()
        };
        await repository.AddAsync(task);
        await repository.SaveChangesAsync();
        return (await repository.GetDetailsAsync(task.TaskId))!.ToDto();
    }

    public async Task<TaskDto> UpdateAsync(int id, TaskRequest request)
    {
        var tagIds = await ValidateReferencesAsync(request);
        var task = await repository.GetForUpdateAsync(id) ?? throw ServiceException.NotFound("Task", id);
        task.Title = request.Title.Trim();
        task.Description = Clean(request.Description);
        task.Status = request.Status;
        task.Priority = request.Priority;
        task.DueDate = request.DueDate;
        task.ProjectId = request.ProjectId;
        task.IsActive = request.IsActive;
        task.ModifiedDate = Now();
        repository.ReplaceTags(task, tagIds);
        await repository.SaveChangesAsync();
        return (await repository.GetDetailsAsync(id))!.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        var task = await repository.GetForUpdateAsync(id) ?? throw ServiceException.NotFound("Task", id);
        task.IsActive = false;
        task.ModifiedDate = Now();
        await repository.SaveChangesAsync();
    }

    private async Task<IReadOnlyList<int>> ValidateReferencesAsync(TaskRequest request)
    {
        if (!await repository.ProjectExistsAsync(request.ProjectId))
            throw ServiceException.BadRequest("projectId", "The selected active project does not exist.");

        var tagIds = request.TagIds.Distinct().ToList();
        if (tagIds.Any(x => x <= 0))
            throw ServiceException.BadRequest("tagIds", "Tag IDs must be positive integers.");
        var existing = await repository.GetExistingTagIdsAsync(tagIds);
        var missing = tagIds.Except(existing).ToList();
        if (missing.Count > 0)
            throw ServiceException.BadRequest("tagIds", $"The following tags do not exist: {string.Join(", ", missing)}.");
        return tagIds;
    }

    private static void ValidateRange(short? value, string field)
    {
        if (value is < 0 or > 3) throw ServiceException.BadRequest(field, $"{field} must be between 0 and 3.");
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static DateTime Now() => DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
}
