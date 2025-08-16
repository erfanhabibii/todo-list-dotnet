using Todo.Core.Entities;

namespace Todo.Core.Repositories;

public interface ITodoRepository
{
    Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TodoItem?> GetByIdOwnedAsync(int id, string ownerId, CancellationToken ct = default);

    IQueryable<TodoItem> QueryAllForAdmin(bool? isDone = null);
    IQueryable<TodoItem> QueryByOwner(string ownerId, bool? isDone = null);

    Task AddAsync(TodoItem item, CancellationToken ct = default);
    void Update(TodoItem item);
    void Delete(TodoItem item);
}
