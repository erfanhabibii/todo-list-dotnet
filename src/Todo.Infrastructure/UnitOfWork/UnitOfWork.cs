using Todo.Core.Repositories;
using Todo.Core.UnitOfWork;
using Todo.Infrastructure.Data;
using Todo.Infrastructure.Repositories;

namespace Todo.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public ITodoRepository Todos { get; }

    public UnitOfWork(AppDbContext db)
    {
        _db = db;
        Todos = new TodoRepository(db);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return _db.SaveChangesAsync(ct);
    }

    public async ValueTask DisposeAsync()
    {
        await _db.DisposeAsync();
    }
}
