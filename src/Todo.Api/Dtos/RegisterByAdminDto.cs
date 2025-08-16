using System.ComponentModel.DataAnnotations;

namespace Todo.Api.Dtos;

public class RegisterByAdminDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email format is invalid.")]
    [MaxLength(256, ErrorMessage = "Email must be at most 256 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be 8-100 characters.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).+$",
        ErrorMessage = "Password must contain upper, lower, digit, and special character.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required.")]
    [RegularExpression(@"^(User|SuperAdmin)$", ErrorMessage = "Invalid role.")]
    public string Role { get; set; } = string.Empty;
}
