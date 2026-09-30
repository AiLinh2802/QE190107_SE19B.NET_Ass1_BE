using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Data;
using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public class TaskRepository(TaskTrackDbContext dbContext) : ITaskRepository
{
    private IQueryable<TaskItem> ActiveQuery() => dbContext.Tasks.AsNoTracking()
        .Include(x => x.Project).ThenInclude(x => x.Department)
        .Include(x => x.TaskTags).ThenInclude(x => x.Tag)
        .Where(x => x.IsActive);

    public async Task<IReadOnlyList<TaskItem>> GetActiveAsync() =>
        await ActiveQuery().OrderBy(x => x.DueDate).ThenBy(x => x.Title).ToListAsync();

    public async Task<IReadOnlyList<TaskItem>> GetByProjectAsync(int projectId) =>
        await ActiveQuery().Where(x => x.ProjectId == projectId).OrderBy(x => x.DueDate).ThenBy(x => x.Title).ToListAsync();

    public async Task<IReadOnlyList<TaskItem>> SearchAsync(string? title, short? status, short? priority, int? projectId, int? tagId)
    {
        var query = ActiveQuery();
        if (!string.IsNullOrWhiteSpace(title)) query = query.Where(x => EF.Functions.ILike(x.Title, $"%{title.Trim()}%"));
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        if (priority.HasValue) query = query.Where(x => x.Priority == priority.Value);
        if (projectId.HasValue) query = query.Where(x => x.ProjectId == projectId.Value);
        if (tagId.HasValue) query = query.Where(x => x.TaskTags.Any(tt => tt.TagId == tagId.Value));
        return await query.OrderBy(x => x.DueDate).ThenBy(x => x.Title).ToListAsync();
    }

    public async Task<TaskItem?> GetDetailsAsync(int id) =>
        await ActiveQuery().FirstOrDefaultAsync(x => x.TaskId == id);

    public Task<TaskItem?> GetForUpdateAsync(int id) => dbContext.Tasks
        .Include(x => x.TaskTags).FirstOrDefaultAsync(x => x.TaskId == id);

    public Task<bool> ProjectExistsAsync(int id) => dbContext.Projects.AnyAsync(x => x.ProjectId == id && x.IsActive);

    public async Task<IReadOnlyList<int>> GetExistingTagIdsAsync(IEnumerable<int> ids) =>
        await dbContext.Tags.Where(x => ids.Contains(x.TagId)).Select(x => x.TagId).ToListAsync();

    public Task AddAsync(TaskItem task) => dbContext.Tasks.AddAsync(task).AsTask();

    public void ReplaceTags(TaskItem task, IEnumerable<int> tagIds)
    {
        dbContext.TaskTags.RemoveRange(task.TaskTags);
        task.TaskTags = tagIds.Select(tagId => new TaskTag { TaskId = task.TaskId, TagId = tagId }).ToList();
    }

    public Task SaveChangesAsync() => dbContext.SaveChangesAsync();
}
