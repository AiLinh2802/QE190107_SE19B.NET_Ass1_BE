using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Data;
using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public class DepartmentRepository(TaskTrackDbContext dbContext) : IDepartmentRepository
{
    public async Task<IReadOnlyList<Department>> GetActiveAsync() =>
        await dbContext.Departments.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.DepartmentName).ToListAsync();

    public async Task<IReadOnlyList<Department>> SearchAsync(string name) =>
        await dbContext.Departments.AsNoTracking()
            .Where(x => x.IsActive && EF.Functions.ILike(x.DepartmentName, $"%{name}%"))
            .OrderBy(x => x.DepartmentName).ToListAsync();

    public async Task<Department?> GetDetailsAsync(int id) =>
        await dbContext.Departments.AsNoTracking()
            .Include(x => x.Projects.Where(p => p.IsActive))
            .FirstOrDefaultAsync(x => x.DepartmentId == id && x.IsActive);

    public Task<Department?> GetForUpdateAsync(int id) => dbContext.Departments.FirstOrDefaultAsync(x => x.DepartmentId == id);
    public Task<bool> HasProjectsAsync(int id) => dbContext.Projects.AnyAsync(x => x.DepartmentId == id);
    public Task AddAsync(Department department) => dbContext.Departments.AddAsync(department).AsTask();
    public void Remove(Department department) => dbContext.Departments.Remove(department);
    public Task SaveChangesAsync() => dbContext.SaveChangesAsync();
}
