using TaskTrack.Service.Contracts;

namespace TaskTrack.Service.Interfaces;

public interface IProjectService
{
    Task<IReadOnlyList<ProjectDto>> GetAllAsync();
    Task<IReadOnlyList<ProjectDto>> GetByDepartmentAsync(int departmentId);
    Task<IReadOnlyList<ProjectDto>> SearchAsync(string? name, short? status, int? departmentId);
    Task<ProjectDetailsDto> GetByIdAsync(int id);
    Task<ProjectDto> CreateAsync(ProjectRequest request);
    Task<ProjectDto> UpdateAsync(int id, ProjectRequest request);
    Task DeleteAsync(int id);
}
