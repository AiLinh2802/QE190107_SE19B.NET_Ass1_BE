using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories;
using TaskTrack.Service.Contracts;
using TaskTrack.Service.Exceptions;
using TaskTrack.Service.Interfaces;
using TaskTrack.Service.Mapping;

namespace TaskTrack.Service.Services;

public class DepartmentService(IDepartmentRepository repository) : IDepartmentService
{
    public async Task<IReadOnlyList<DepartmentDto>> GetAllAsync() =>
        (await repository.GetActiveAsync()).Select(x => x.ToDto()).ToList();

    public async Task<IReadOnlyList<DepartmentDto>> SearchAsync(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return await GetAllAsync();
        return (await repository.SearchAsync(name.Trim())).Select(x => x.ToDto()).ToList();
    }

    public async Task<DepartmentDetailsDto> GetByIdAsync(int id)
    {
        var department = await repository.GetDetailsAsync(id) ?? throw ServiceException.NotFound("Department", id);
        return department.ToDetailsDto();
    }

    public async Task<DepartmentDto> CreateAsync(DepartmentRequest request)
    {
        var department = new Department
        {
            DepartmentName = request.DepartmentName.Trim(),
            DepartmentDescription = request.DepartmentDescription.Trim(),
            IsActive = request.IsActive
        };
        await repository.AddAsync(department);
        await repository.SaveChangesAsync();
        return department.ToDto();
    }

    public async Task<DepartmentDto> UpdateAsync(int id, DepartmentRequest request)
    {
        var department = await repository.GetForUpdateAsync(id) ?? throw ServiceException.NotFound("Department", id);
        department.DepartmentName = request.DepartmentName.Trim();
        department.DepartmentDescription = request.DepartmentDescription.Trim();
        department.IsActive = request.IsActive;
        await repository.SaveChangesAsync();
        return department.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        var department = await repository.GetForUpdateAsync(id) ?? throw ServiceException.NotFound("Department", id);
        if (await repository.HasProjectsAsync(id))
            throw ServiceException.BadRequest("departmentId", "The department cannot be deleted because projects are linked to it.");
        repository.Remove(department);
        await repository.SaveChangesAsync();
    }
}
