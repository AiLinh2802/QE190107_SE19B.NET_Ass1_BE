using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public interface ITagRepository
{
    Task<IReadOnlyList<Tag>> GetAllAsync();
    Task<Tag?> GetForUpdateAsync(int id);
    Task<bool> NameExistsAsync(string name, int? excludingId = null);
    Task<bool> IsUsedAsync(int id);
    Task AddAsync(Tag tag);
    void Remove(Tag tag);
    Task SaveChangesAsync();
}
