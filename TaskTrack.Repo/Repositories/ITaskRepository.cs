using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public interface ITaskRepository
{
    Task<IReadOnlyList<TaskItem>> GetActiveAsync();
    Task<IReadOnlyList<TaskItem>> GetByProjectAsync(int projectId);
    Task<IReadOnlyList<TaskItem>> SearchAsync(string? title, short? status, short? priority, int? projectId, int? tagId);
    Task<TaskItem?> GetDetailsAsync(int id);
    Task<TaskItem?> GetForUpdateAsync(int id);
    Task<bool> ProjectExistsAsync(int id);
    Task<IReadOnlyList<int>> GetExistingTagIdsAsync(IEnumerable<int> ids);
    Task AddAsync(TaskItem task);
    void ReplaceTags(TaskItem task, IEnumerable<int> tagIds);
    Task SaveChangesAsync();
}
