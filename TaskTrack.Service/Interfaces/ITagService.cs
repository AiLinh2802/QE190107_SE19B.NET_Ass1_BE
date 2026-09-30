using TaskTrack.Service.Contracts;

namespace TaskTrack.Service.Interfaces;

public interface ITagService
{
    Task<IReadOnlyList<TagDto>> GetAllAsync();
    Task<TagDto> CreateAsync(TagRequest request);
    Task<TagDto> UpdateAsync(int id, TagRequest request);
    Task DeleteAsync(int id);
}
