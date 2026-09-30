using TaskTrack.Service.Contracts;

namespace TaskTrack.Service.Interfaces;

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync();
    Task<IReadOnlyList<DepartmentDto>> SearchAsync(string? name);
    Task<DepartmentDetailsDto> GetByIdAsync(int id);
    Task<DepartmentDto> CreateAsync(DepartmentRequest request);
    Task<DepartmentDto> UpdateAsync(int id, DepartmentRequest request);
    Task DeleteAsync(int id);
}
