using Todo.Core.Entities;

namespace Todo.Core.Repositories;

public interface ITodoRepository
{
    Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<TodoItem>> GetOpenAsync(CancellationToken ct = default);

    Task<IReadOnlyList<TodoItem>> GetAllByOwnerAsync(
        string ownerId,
        bool? isDone = null,
        CancellationToken ct = default);

    Task<TodoItem?> GetByIdOwnedAsync(
        int id,
        string ownerId,
        CancellationToken ct = default);

    Task AddAsync(TodoItem item, CancellationToken ct = default);
    void Update(TodoItem item);
    void Delete(TodoItem item);
}
