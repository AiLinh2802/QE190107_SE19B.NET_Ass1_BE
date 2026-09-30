using TaskTrack.Service.Contracts;

namespace TaskTrack.Service.Interfaces;

public interface ITaskService
{
    Task<IReadOnlyList<TaskDto>> GetAllAsync();
    Task<IReadOnlyList<TaskDto>> GetByProjectAsync(int projectId);
    Task<IReadOnlyList<TaskDto>> SearchAsync(string? title, short? status, short? priority, int? projectId, int? tagId);
    Task<TaskDto> GetByIdAsync(int id);
    Task<TaskDto> CreateAsync(TaskRequest request);
    Task<TaskDto> UpdateAsync(int id, TaskRequest request);
    Task DeleteAsync(int id);
}
