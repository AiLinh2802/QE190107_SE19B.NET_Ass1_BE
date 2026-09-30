using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.Contracts;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.API.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController(IDepartmentService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetAll() => Ok(await service.GetAllAsync());
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartmentDetailsDto>> GetById(int id) => Ok(await service.GetByIdAsync(id));
    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> Search([FromQuery] string? name) => Ok(await service.SearchAsync(name));
    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> Create(DepartmentRequest request)
    {
        var created = await service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.DepartmentId }, created);
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult<DepartmentDto>> Update(int id, DepartmentRequest request) => Ok(await service.UpdateAsync(id, request));
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { await service.DeleteAsync(id); return NoContent(); }
}
