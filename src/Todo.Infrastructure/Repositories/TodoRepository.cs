using Microsoft.EntityFrameworkCore;
using Todo.Core.Entities;
using Todo.Core.Repositories;
using Todo.Infrastructure.Data;

namespace Todo.Infrastructure.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly AppDbContext _db;

    public TodoRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Set<TodoItem>().FindAsync(new object[] { id }, ct);

    public async Task<TodoItem?> GetByIdOwnedAsync(int id, string ownerId, CancellationToken ct = default)
        => await _db.Set<TodoItem>()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == ownerId, ct);

    public IQueryable<TodoItem> QueryAllForAdmin(bool? isDone = null)
    {
        var q = _db.Set<TodoItem>().AsNoTracking().AsQueryable();
        if (isDone.HasValue) q = q.Where(t => t.IsDone == isDone.Value);
        return q.OrderBy(t => t.CreatedAtUtc);
    }

    public IQueryable<TodoItem> QueryByOwner(string ownerId, bool? isDone = null)
    {
        var q = _db.Set<TodoItem>().AsNoTracking().Where(t => t.OwnerId == ownerId);
        if (isDone.HasValue) q = q.Where(t => t.IsDone == isDone.Value);
        return q.OrderBy(t => t.CreatedAtUtc);
    }

    public async Task AddAsync(TodoItem item, CancellationToken ct = default)
        => await _db.Set<TodoItem>().AddAsync(item, ct);

    public void Update(TodoItem item) => _db.Set<TodoItem>().Update(item);
    public void Delete(TodoItem item) => _db.Set<TodoItem>().Remove(item);
}
