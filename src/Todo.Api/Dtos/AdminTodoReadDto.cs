namespace Todo.Api.Dtos;

public class AdminTodoReadDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string OwnerId { get; set; } = string.Empty;
}
