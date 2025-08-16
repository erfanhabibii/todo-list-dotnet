using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Todo.Api.Dtos;
using Todo.Infrastructure.Auth;
using Todo.Infrastructure.Data;

namespace Todo.Api.Services;

public class AuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;
    private readonly AppDbContext _db;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config, AppDbContext db)
    {
        _userManager = userManager;
        _config = config;
        _db = db;
    }

    public async Task<IdentityResult> RegisterSelfAsync(RegisterSelfDto dto)
    {
        var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email, EmailConfirmed = true };

        await using var tx = await _db.Database.BeginTransactionAsync();

        var create = await _userManager.CreateAsync(user, dto.Password);
        if (!create.Succeeded)
        {
            await tx.RollbackAsync();
            return create;
        }

        var addRole = await _userManager.AddToRoleAsync(user, AppRoles.User);
        if (!addRole.Succeeded)
        {
            await tx.RollbackAsync();
            return IdentityResult.Failed(addRole.Errors.ToArray());
        }

        await tx.CommitAsync();
        return IdentityResult.Success;
    }

    public async Task<(IdentityResult Result, string? UserId, string AssignedRole)> RegisterByAdminAsync(RegisterByAdminDto dto)
    {
        var role = dto.Role;

        var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email, EmailConfirmed = true };

        await using var tx = await _db.Database.BeginTransactionAsync();

        var create = await _userManager.CreateAsync(user, dto.Password);
        if (!create.Succeeded)
        {
            await tx.RollbackAsync();
            return (create, null, role);
        }

        var addRole = await _userManager.AddToRoleAsync(user, role);
        if (!addRole.Succeeded)
        {
            await tx.RollbackAsync();
            return (IdentityResult.Failed(addRole.Errors.ToArray()), null, role);
        }

        await tx.CommitAsync();
        return (IdentityResult.Success, user.Id, role);
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return null;

        var valid = await _userManager.CheckPasswordAsync(user, password);
        if (!valid) return null;

        return await GenerateJwtTokenAsync(user);
    }

    private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        var jwtSection = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!)
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var r in roles)
            claims.Add(new Claim(ClaimTypes.Role, r));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
