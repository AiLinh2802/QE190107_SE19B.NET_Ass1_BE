using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Data;
using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public class ProjectRepository(TaskTrackDbContext dbContext) : IProjectRepository
{
    private IQueryable<Project> ActiveQuery() => dbContext.Projects.AsNoTracking().Include(x => x.Department).Where(x => x.IsActive);

    public async Task<IReadOnlyList<Project>> GetActiveAsync() =>
        await ActiveQuery().OrderBy(x => x.ProjectName).ToListAsync();

    public async Task<IReadOnlyList<Project>> GetByDepartmentAsync(int departmentId) =>
        await ActiveQuery().Where(x => x.DepartmentId == departmentId).OrderBy(x => x.ProjectName).ToListAsync();

    public async Task<IReadOnlyList<Project>> SearchAsync(string? name, short? status, int? departmentId)
    {
        var query = ActiveQuery();
        if (!string.IsNullOrWhiteSpace(name)) query = query.Where(x => EF.Functions.ILike(x.ProjectName, $"%{name.Trim()}%"));
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        if (departmentId.HasValue) query = query.Where(x => x.DepartmentId == departmentId.Value);
        return await query.OrderBy(x => x.ProjectName).ToListAsync();
    }

    public async Task<Project?> GetDetailsAsync(int id) =>
        await dbContext.Projects.AsNoTracking().Include(x => x.Department)
            .Include(x => x.Tasks.Where(t => t.IsActive)).ThenInclude(t => t.TaskTags).ThenInclude(tt => tt.Tag)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.ProjectId == id && x.IsActive);

    public Task<Project?> GetForUpdateAsync(int id) => dbContext.Projects.FirstOrDefaultAsync(x => x.ProjectId == id);
    public Task<bool> DepartmentExistsAsync(int id) => dbContext.Departments.AnyAsync(x => x.DepartmentId == id && x.IsActive);
    public Task<bool> HasTasksAsync(int id) => dbContext.Tasks.AnyAsync(x => x.ProjectId == id);
    public Task AddAsync(Project project) => dbContext.Projects.AddAsync(project).AsTask();
    public void Remove(Project project) => dbContext.Projects.Remove(project);
    public Task SaveChangesAsync() => dbContext.SaveChangesAsync();
}
