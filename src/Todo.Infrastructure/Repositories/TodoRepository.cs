using Microsoft.EntityFrameworkCore;
using Todo.Core.Entities;
using Todo.Core.Repositories;
using Todo.Infrastructure.Data;

namespace Todo.Infrastructure.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly AppDbContext _db;
    private readonly DbSet<TodoItem> _set;

    public TodoRepository(AppDbContext db)
    {
        _db = db;
        _set = db.Set<TodoItem>();
    }

    public async Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _set.FindAsync(new object[] { id }, ct);

    public async Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken ct = default)
        => await _set.AsNoTracking().OrderBy(t => t.CreatedAtUtc).ToListAsync(ct);

    public async Task<IReadOnlyList<TodoItem>> GetOpenAsync(CancellationToken ct = default)
        => await _set.AsNoTracking().Where(t => !t.IsDone).OrderBy(t => t.CreatedAtUtc).ToListAsync(ct);

    public async Task<IReadOnlyList<TodoItem>> GetAllByOwnerAsync(string ownerId, bool? isDone = null, CancellationToken ct = default)
    {
        var query = _set.AsNoTracking().Where(t => t.OwnerId == ownerId);
        if (isDone.HasValue) query = query.Where(t => t.IsDone == isDone.Value);
        return await query.OrderBy(t => t.CreatedAtUtc).ToListAsync(ct);
    }

    public async Task<TodoItem?> GetByIdOwnedAsync(int id, string ownerId, CancellationToken ct = default)
        => await _set.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == ownerId, ct);

    public async Task AddAsync(TodoItem item, CancellationToken ct = default)
        => await _set.AddAsync(item, ct);

    public void Update(TodoItem item) => _set.Update(item);

    public void Delete(TodoItem item) => _set.Remove(item);
}
