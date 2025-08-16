namespace Todo.Api.Dtos;

public class TodoReadDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
