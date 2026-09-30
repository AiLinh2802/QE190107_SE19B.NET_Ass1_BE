using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.Contracts;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.API.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController(IProjectService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetAll() => Ok(await service.GetAllAsync());
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDetailsDto>> GetById(int id) => Ok(await service.GetByIdAsync(id));
    [HttpGet("department/{departmentId:int}")]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetByDepartment(int departmentId) => Ok(await service.GetByDepartmentAsync(departmentId));
    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> Search([FromQuery] string? name, [FromQuery] short? status, [FromQuery] int? departmentId) => Ok(await service.SearchAsync(name, status, departmentId));
    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(ProjectRequest request)
    {
        var created = await service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.ProjectId }, created);
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProjectDto>> Update(int id, ProjectRequest request) => Ok(await service.UpdateAsync(id, request));
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { await service.DeleteAsync(id); return NoContent(); }
}
