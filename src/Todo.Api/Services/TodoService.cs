using System.Security.Claims;
using Todo.Core.Entities;
using Todo.Core.UnitOfWork;
using Todo.Api.Dtos;
using Todo.Infrastructure.Auth;
using AutoMapper;
using Todo.Core.Auth;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace Todo.Api.Services;

public class TodoService
{
    private const int MaxPageSize = 100;
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _http;
    private readonly IMapper _mapper;

    public TodoService(IUnitOfWork uow, IHttpContextAccessor http, IMapper mapper)
    {
        _uow = uow;
        _http = http;
        _mapper = mapper;
    }

    private string GetCurrentUserId()
    {
        return _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? throw new InvalidOperationException("User is not authenticated.");
    }

    private bool IsSuperAdmin()
        => _http.HttpContext?.User.IsInRole(AppRoles.SuperAdmin) == true;

    private static int ClampTake(int take) =>
        take <= 0 ? 20 : (take > MaxPageSize ? MaxPageSize : take);

    public Task<List<AdminTodoReadDto>> GetAllForAdminAsync(bool? isDone, int skip, int take, CancellationToken ct = default)
    {
        var query = _uow.Todos.QueryAllForAdmin(isDone);
        return query
            .Skip(skip < 0 ? 0 : skip)
            .Take(ClampTake(take))
            .ProjectTo<AdminTodoReadDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public Task<List<TodoReadDto>> GetAllForUserAsync(bool? isDone, int skip, int take, CancellationToken ct = default)
    {
        var ownerId = GetCurrentUserId();
        var query = _uow.Todos.QueryByOwner(ownerId, isDone);
        return query
            .Skip(skip < 0 ? 0 : skip)
            .Take(ClampTake(take))
            .ProjectTo<TodoReadDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<TodoReadDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        if (IsSuperAdmin())
        {
            var any = await _uow.Todos.GetByIdAsync(id, ct);
            return any is null ? null : _mapper.Map<TodoReadDto>(any);
        }

        var ownerId = GetCurrentUserId();
        var item = await _uow.Todos.GetByIdOwnedAsync(id, ownerId, ct);
        return item is null ? null : _mapper.Map<TodoReadDto>(item);
    }

    public async Task<TodoReadDto> CreateAsync(CreateTodoDto dto, CancellationToken ct = default)
    {
        var ownerId = GetCurrentUserId();

        var todo = _mapper.Map<TodoItem>(dto);
        todo.OwnerId = ownerId;

        await _uow.Todos.AddAsync(todo, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<TodoReadDto>(todo);
    }

    public async Task<TodoReadDto?> UpdateAsync(int id, UpdateTodoDto dto, CancellationToken ct = default)
    {
        var entity = IsSuperAdmin()
            ? await _uow.Todos.GetByIdAsync(id, ct)
            : await _uow.Todos.GetByIdOwnedAsync(id, GetCurrentUserId(), ct);

        if (entity is null) return null;

        _mapper.Map(dto, entity);
        _uow.Todos.Update(entity);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<TodoReadDto>(entity);
    }

    public async Task<TodoReadDto?> PatchAsync(int id, PatchTodoDto dto, CancellationToken ct = default)
    {
        var entity = IsSuperAdmin()
            ? await _uow.Todos.GetByIdAsync(id, ct)
            : await _uow.Todos.GetByIdOwnedAsync(id, GetCurrentUserId(), ct);

        if (entity is null) return null;

        _mapper.Map(dto, entity);
        _uow.Todos.Update(entity);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<TodoReadDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = IsSuperAdmin()
            ? await _uow.Todos.GetByIdAsync(id, ct)
            : await _uow.Todos.GetByIdOwnedAsync(id, GetCurrentUserId(), ct);

        if (entity is null) return false;

        _uow.Todos.Delete(entity);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}
