using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Dtos;
using Todo.Api.Services;
using Todo.Core.Auth;

namespace Todo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register-self")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterSelf(RegisterSelfDto dto)
    {
        var result = await _authService.RegisterSelfAsync(dto);
        if (!result.Succeeded) return BadRequest(result.Errors);
        return Ok(new { message = "User registered successfully" });
    }

    [HttpPost("register")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public async Task<IActionResult> RegisterByAdmin(RegisterByAdminDto dto)
    {
        var (result, userId, role) = await _authService.RegisterByAdminAsync(dto);
        if (!result.Succeeded) return BadRequest(result.Errors);
        return Ok(new { message = "User registered by admin", userId, role });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto.Email, dto.Password);
        if (token is null) return Unauthorized(new { message = "Invalid credentials" });
        return Ok(new { token });
    }
}
