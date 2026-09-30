using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public interface IProjectRepository
{
    Task<IReadOnlyList<Project>> GetActiveAsync();
    Task<IReadOnlyList<Project>> GetByDepartmentAsync(int departmentId);
    Task<IReadOnlyList<Project>> SearchAsync(string? name, short? status, int? departmentId);
    Task<Project?> GetDetailsAsync(int id);
    Task<Project?> GetForUpdateAsync(int id);
    Task<bool> DepartmentExistsAsync(int id);
    Task<bool> HasTasksAsync(int id);
    Task AddAsync(Project project);
    void Remove(Project project);
    Task SaveChangesAsync();
}
