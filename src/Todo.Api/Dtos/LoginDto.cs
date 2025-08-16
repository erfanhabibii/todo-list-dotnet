using System.ComponentModel.DataAnnotations;

namespace Todo.Api.Dtos;

public class LoginDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email.")]
    [MaxLength(256, ErrorMessage = "Email must be at most 256 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be 8-100 characters.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W)[^\s]+$", ErrorMessage = "Password must include upper, lower, digit, special char, and no whitespace.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
