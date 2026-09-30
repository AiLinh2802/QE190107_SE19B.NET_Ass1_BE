using TaskTrack.Repo.Models;
using TaskTrack.Service.Contracts;

namespace TaskTrack.Service.Mapping;

internal static class DtoMapper
{
    public static DepartmentDto ToDto(this Department entity) =>
        new(entity.DepartmentId, entity.DepartmentName, entity.DepartmentDescription, entity.IsActive);

    public static DepartmentDetailsDto ToDetailsDto(this Department entity) =>
        new(entity.DepartmentId, entity.DepartmentName, entity.DepartmentDescription, entity.IsActive,
            entity.Projects.OrderBy(x => x.ProjectName).Select(x => new ProjectDto(
                x.ProjectId, x.ProjectName, x.Description, x.StartDate, x.EndDate, x.Status,
                x.DepartmentId, entity.DepartmentName, x.IsActive, x.CreatedDate)).ToList());

    public static ProjectDto ToDto(this Project entity) =>
        new(entity.ProjectId, entity.ProjectName, entity.Description, entity.StartDate, entity.EndDate,
            entity.Status, entity.DepartmentId, entity.Department?.DepartmentName ?? string.Empty,
            entity.IsActive, entity.CreatedDate);

    public static ProjectDetailsDto ToDetailsDto(this Project entity) =>
        new(entity.ProjectId, entity.ProjectName, entity.Description, entity.StartDate, entity.EndDate,
            entity.Status, entity.DepartmentId, entity.Department.DepartmentName, entity.IsActive,
            entity.CreatedDate, entity.Tasks.OrderBy(x => x.DueDate).ThenBy(x => x.Title).Select(x => x.ToDto()).ToList());

    public static TagDto ToDto(this Tag entity) => new(entity.TagId, entity.TagName, entity.Color);

    public static TaskDto ToDto(this TaskItem entity) =>
        new(entity.TaskId, entity.Title, entity.Description, entity.Status, entity.Priority, entity.DueDate,
            entity.ProjectId, entity.Project?.ProjectName ?? string.Empty, entity.IsActive, entity.CreatedDate,
            entity.ModifiedDate, entity.TaskTags.Select(x => x.Tag.ToDto()).OrderBy(x => x.TagName).ToList());
}
