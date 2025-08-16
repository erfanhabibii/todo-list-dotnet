using System.ComponentModel.DataAnnotations;

namespace Todo.Api.Dtos;

public class UpdateTodoDto
{
    [Required(ErrorMessage = "Title is required.")]
    [MinLength(3, ErrorMessage = "Title must be at least 3 characters.")]
    [MaxLength(200, ErrorMessage = "Title must be at most 200 characters.")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "IsDone is required.")]
    public bool IsDone { get; set; }
}
