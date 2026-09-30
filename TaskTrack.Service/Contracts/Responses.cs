namespace TaskTrack.Service.Contracts;

public record DepartmentDto(int DepartmentId, string DepartmentName, string DepartmentDescription, bool IsActive);
public record DepartmentDetailsDto(int DepartmentId, string DepartmentName, string DepartmentDescription, bool IsActive, IReadOnlyList<ProjectDto> Projects);

public record ProjectDto(
    int ProjectId,
    string ProjectName,
    string? Description,
    DateOnly StartDate,
    DateOnly? EndDate,
    short Status,
    int DepartmentId,
    string DepartmentName,
    bool IsActive,
    DateTime CreatedDate);

public record ProjectDetailsDto(
    int ProjectId,
    string ProjectName,
    string? Description,
    DateOnly StartDate,
    DateOnly? EndDate,
    short Status,
    int DepartmentId,
    string DepartmentName,
    bool IsActive,
    DateTime CreatedDate,
    IReadOnlyList<TaskDto> Tasks);

public record TagDto(int TagId, string TagName, string? Color);

public record TaskDto(
    int TaskId,
    string Title,
    string? Description,
    short Status,
    short Priority,
    DateOnly? DueDate,
    int ProjectId,
    string ProjectName,
    bool IsActive,
    DateTime CreatedDate,
    DateTime? ModifiedDate,
    IReadOnlyList<TagDto> Tags);
