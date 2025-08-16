using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Dtos;
using Todo.Api.Services;
using Todo.Core.Entities;

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
    public async Task<ActionResult<IReadOnlyList<TodoItem>>> GetAll([FromQuery] bool? isDone, CancellationToken ct)
    {
        var todos = await _service.GetAllAsync(isDone, ct);
        return Ok(todos);
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> Create(CreateTodoDto dto, CancellationToken ct)
    {
        var created = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoItem>> GetById(int id, CancellationToken ct)
    {
        var todo = await _service.GetByIdAsync(id, ct);
        return todo is null ? NotFound(new { message = "Todo not found." }) : Ok(todo);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TodoItem>> Update(int id, UpdateTodoDto dto, CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(id, dto, ct);
        return updated is null ? NotFound(new { message = "Todo not found." }) : Ok(updated);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<TodoItem>> Patch(int id, PatchTodoDto dto, CancellationToken ct)
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
