using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public interface IDepartmentRepository
{
    Task<IReadOnlyList<Department>> GetActiveAsync();
    Task<IReadOnlyList<Department>> SearchAsync(string name);
    Task<Department?> GetDetailsAsync(int id);
    Task<Department?> GetForUpdateAsync(int id);
    Task<bool> HasProjectsAsync(int id);
    Task AddAsync(Department department);
    void Remove(Department department);
    Task SaveChangesAsync();
}
