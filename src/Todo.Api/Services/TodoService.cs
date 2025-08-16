using System.Security.Claims;
using Todo.Core.Entities;
using Todo.Core.UnitOfWork;
using Todo.Api.Dtos;

namespace Todo.Api.Services;

public class TodoService
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;

    public TodoService(IUnitOfWork uow, IHttpContextAccessor http)
    {
        _uow = uow;
        _http = http;
    }

    private string GetCurrentUserId()
    {
        return _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? throw new InvalidOperationException("User is not authenticated.");
    }

    public Task<IReadOnlyList<TodoItem>> GetAllAsync(bool? isDone = null, CancellationToken ct = default)
    {
        var ownerId = GetCurrentUserId();
        return _uow.Todos.GetAllByOwnerAsync(ownerId, isDone, ct);
    }

    public async Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var ownerId = GetCurrentUserId();
        return await _uow.Todos.GetByIdOwnedAsync(id, ownerId, ct);
    }

    public async Task<TodoItem> CreateAsync(CreateTodoDto dto, CancellationToken ct = default)
    {
        var ownerId = GetCurrentUserId();

        var todo = new TodoItem
        {
            Title = dto.Title,
            CreatedAtUtc = DateTime.UtcNow,
            IsDone = false,
            OwnerId = ownerId
        };

        await _uow.Todos.AddAsync(todo, ct);
        await _uow.SaveChangesAsync(ct);

        return todo;
    }

    public async Task<TodoItem?> UpdateAsync(int id, UpdateTodoDto dto, CancellationToken ct = default)
    {
        var ownerId = GetCurrentUserId();
        var current = await _uow.Todos.GetByIdOwnedAsync(id, ownerId, ct);
        if (current is null) return null;

        current.Title = dto.Title;
        current.IsDone = dto.IsDone;

        _uow.Todos.Update(current);
        await _uow.SaveChangesAsync(ct);
        return current;
    }

    public async Task<TodoItem?> PatchAsync(int id, PatchTodoDto dto, CancellationToken ct = default)
    {
        var ownerId = GetCurrentUserId();
        var current = await _uow.Todos.GetByIdOwnedAsync(id, ownerId, ct);
        if (current is null) return null;

        if (dto.Title is not null) current.Title = dto.Title;
        if (dto.IsDone.HasValue) current.IsDone = dto.IsDone.Value;

        _uow.Todos.Update(current);
        await _uow.SaveChangesAsync(ct);
        return current;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var ownerId = GetCurrentUserId();
        var current = await _uow.Todos.GetByIdOwnedAsync(id, ownerId, ct);
        if (current is null) return false;

        _uow.Todos.Delete(current);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}
