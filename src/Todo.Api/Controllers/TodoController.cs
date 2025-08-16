using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Dtos;
using Todo.Api.Services;
using Todo.Core.Entities;
using Todo.Core.Auth;

namespace Todo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TodoController : ControllerBase
{
    private readonly TodoService _service;

    public TodoController(TodoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TodoReadDto>>> GetAll(
        [FromQuery] bool? isDone,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20,
        CancellationToken ct = default)
    {
        var items = await _service.GetAllForUserAsync(isDone, skip, take, ct);
        return Ok(items);
    }

    [HttpGet("admin")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<ActionResult<IReadOnlyList<AdminTodoReadDto>>> GetAllAdmin(
        [FromQuery] bool? isDone,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20,
        CancellationToken ct = default)
    {
        var items = await _service.GetAllForAdminAsync(isDone, skip, take, ct);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<TodoReadDto>> Create(CreateTodoDto dto, CancellationToken ct)
    {
        var created = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoReadDto>> GetById(int id, CancellationToken ct)
    {
        var todo = await _service.GetByIdAsync(id, ct);
        return todo is null ? NotFound(new { message = "Todo not found." }) : Ok(todo);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TodoReadDto>> Update(int id, UpdateTodoDto dto, CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(id, dto, ct);
        return updated is null ? NotFound(new { message = "Todo not found." }) : Ok(updated);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<TodoReadDto>> Patch(int id, PatchTodoDto dto, CancellationToken ct)
    {
        var updated = await _service.PatchAsync(id, dto, ct);
        return updated is null ? NotFound(new { message = "Todo not found." }) : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var success = await _service.DeleteAsync(id, ct);
        return success ? NoContent() : NotFound(new { message = "Todo not found." });
    }
}
