using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Data;
using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public class TagRepository(TaskTrackDbContext dbContext) : ITagRepository
{
    public async Task<IReadOnlyList<Tag>> GetAllAsync() =>
        await dbContext.Tags.AsNoTracking().OrderBy(x => x.TagName).ToListAsync();

    public Task<Tag?> GetForUpdateAsync(int id) => dbContext.Tags.FirstOrDefaultAsync(x => x.TagId == id);
    public Task<bool> NameExistsAsync(string name, int? excludingId = null) =>
        dbContext.Tags.AnyAsync(x => x.TagName.ToLower() == name.ToLower() && (!excludingId.HasValue || x.TagId != excludingId.Value));
    public Task<bool> IsUsedAsync(int id) => dbContext.TaskTags.AnyAsync(x => x.TagId == id);
    public Task AddAsync(Tag tag) => dbContext.Tags.AddAsync(tag).AsTask();
    public void Remove(Tag tag) => dbContext.Tags.Remove(tag);
    public Task SaveChangesAsync() => dbContext.SaveChangesAsync();
}
