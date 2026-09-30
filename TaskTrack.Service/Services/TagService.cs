using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories;
using TaskTrack.Service.Contracts;
using TaskTrack.Service.Exceptions;
using TaskTrack.Service.Interfaces;
using TaskTrack.Service.Mapping;

namespace TaskTrack.Service.Services;

public class TagService(ITagRepository repository) : ITagService
{
    public async Task<IReadOnlyList<TagDto>> GetAllAsync() =>
        (await repository.GetAllAsync()).Select(x => x.ToDto()).ToList();

    public async Task<TagDto> CreateAsync(TagRequest request)
    {
        var name = request.TagName.Trim();
        if (await repository.NameExistsAsync(name))
            throw ServiceException.BadRequest("tagName", "A tag with this name already exists.");
        var tag = new Tag { TagName = name, Color = NormalizeColor(request.Color) };
        await repository.AddAsync(tag);
        await repository.SaveChangesAsync();
        return tag.ToDto();
    }

    public async Task<TagDto> UpdateAsync(int id, TagRequest request)
    {
        var tag = await repository.GetForUpdateAsync(id) ?? throw ServiceException.NotFound("Tag", id);
        var name = request.TagName.Trim();
        if (await repository.NameExistsAsync(name, id))
            throw ServiceException.BadRequest("tagName", "A tag with this name already exists.");
        tag.TagName = name;
        tag.Color = NormalizeColor(request.Color);
        await repository.SaveChangesAsync();
        return tag.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        var tag = await repository.GetForUpdateAsync(id) ?? throw ServiceException.NotFound("Tag", id);
        if (await repository.IsUsedAsync(id))
            throw ServiceException.BadRequest("tagId", "The tag cannot be deleted because it is used by one or more tasks.");
        repository.Remove(tag);
        await repository.SaveChangesAsync();
    }

    private static string? NormalizeColor(string? color) => string.IsNullOrWhiteSpace(color) ? null : color.ToUpperInvariant();
}
