using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.Contracts;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.API.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController(ITaskService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetAll() => Ok(await service.GetAllAsync());
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskDto>> GetById(int id) => Ok(await service.GetByIdAsync(id));
    [HttpGet("project/{projectId:int}")]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetByProject(int projectId) => Ok(await service.GetByProjectAsync(projectId));
    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> Search([FromQuery] string? title, [FromQuery] short? status, [FromQuery] short? priority, [FromQuery] int? projectId, [FromQuery] int? tagId) => Ok(await service.SearchAsync(title, status, priority, projectId, tagId));
    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create(TaskRequest request)
    {
        var created = await service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.TaskId }, created);
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskDto>> Update(int id, TaskRequest request) => Ok(await service.UpdateAsync(id, request));
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { await service.DeleteAsync(id); return NoContent(); }
}
