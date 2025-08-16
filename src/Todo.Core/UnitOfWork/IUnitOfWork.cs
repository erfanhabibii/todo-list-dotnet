using Todo.Core.Repositories;

namespace Todo.Core.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    ITodoRepository Todos { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
