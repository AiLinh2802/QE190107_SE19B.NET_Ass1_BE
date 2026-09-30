using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.Contracts;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.API.Controllers;

[ApiController]
[Route("api/tags")]
public class TagsController(ITagService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TagDto>>> GetAll() => Ok(await service.GetAllAsync());
    [HttpPost]
    public async Task<ActionResult<TagDto>> Create(TagRequest request) => StatusCode(StatusCodes.Status201Created, await service.CreateAsync(request));
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TagDto>> Update(int id, TagRequest request) => Ok(await service.UpdateAsync(id, request));
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { await service.DeleteAsync(id); return NoContent(); }
}
