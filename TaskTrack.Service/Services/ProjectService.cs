using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories;
using TaskTrack.Service.Contracts;
using TaskTrack.Service.Exceptions;
using TaskTrack.Service.Interfaces;
using TaskTrack.Service.Mapping;

namespace TaskTrack.Service.Services;

public class ProjectService(IProjectRepository repository) : IProjectService
{
    public async Task<IReadOnlyList<ProjectDto>> GetAllAsync() =>
        (await repository.GetActiveAsync()).Select(x => x.ToDto()).ToList();

    public async Task<IReadOnlyList<ProjectDto>> GetByDepartmentAsync(int departmentId) =>
        (await repository.GetByDepartmentAsync(departmentId)).Select(x => x.ToDto()).ToList();

    public async Task<IReadOnlyList<ProjectDto>> SearchAsync(string? name, short? status, int? departmentId)
    {
        ValidateStatus(status);
        return (await repository.SearchAsync(name, status, departmentId)).Select(x => x.ToDto()).ToList();
    }

    public async Task<ProjectDetailsDto> GetByIdAsync(int id)
    {
        var project = await repository.GetDetailsAsync(id) ?? throw ServiceException.NotFound("Project", id);
        return project.ToDetailsDto();
    }

    public async Task<ProjectDto> CreateAsync(ProjectRequest request)
    {
        await EnsureDepartmentAsync(request.DepartmentId);
        var project = new Project
        {
            ProjectName = request.ProjectName.Trim(),
            Description = Clean(request.Description),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = request.Status,
            DepartmentId = request.DepartmentId,
            IsActive = request.IsActive,
            CreatedDate = Now()
        };
        await repository.AddAsync(project);
        await repository.SaveChangesAsync();
        return (await repository.GetDetailsAsync(project.ProjectId))!.ToDto();
    }

    public async Task<ProjectDto> UpdateAsync(int id, ProjectRequest request)
    {
        await EnsureDepartmentAsync(request.DepartmentId);
        var project = await repository.GetForUpdateAsync(id) ?? throw ServiceException.NotFound("Project", id);
        project.ProjectName = request.ProjectName.Trim();
        project.Description = Clean(request.Description);
        project.StartDate = request.StartDate;
        project.EndDate = request.EndDate;
        project.Status = request.Status;
        project.DepartmentId = request.DepartmentId;
        project.IsActive = request.IsActive;
        await repository.SaveChangesAsync();
        return (await repository.GetDetailsAsync(id))!.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        var project = await repository.GetForUpdateAsync(id) ?? throw ServiceException.NotFound("Project", id);
        if (await repository.HasTasksAsync(id))
            throw ServiceException.BadRequest("projectId", "The project cannot be deleted because tasks are linked to it.");
        repository.Remove(project);
        await repository.SaveChangesAsync();
    }

    private async Task EnsureDepartmentAsync(int id)
    {
        if (!await repository.DepartmentExistsAsync(id))
            throw ServiceException.BadRequest("departmentId", "The selected active department does not exist.");
    }

    private static void ValidateStatus(short? status)
    {
        if (status is < 0 or > 3) throw ServiceException.BadRequest("status", "Status must be between 0 and 3.");
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static DateTime Now() => DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
}
