using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Core.Auth;
using Todo.Infrastructure.Auth;

namespace Todo.Api.Setup;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { AppRoles.SuperAdmin, AppRoles.User })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var email = config["SuperAdminSeed:Email"];
        var password = config["SuperAdminSeed:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var admin = await userManager.FindByEmailAsync(email);
        if (admin == null)
        {
            admin = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
            var create = await userManager.CreateAsync(admin, password);
            if (!create.Succeeded) return;
            await userManager.AddToRoleAsync(admin, AppRoles.SuperAdmin);
        }
        else
        {
            var roles = await userManager.GetRolesAsync(admin);
            if (!roles.Contains(AppRoles.SuperAdmin))
                await userManager.AddToRoleAsync(admin, AppRoles.SuperAdmin);
        }
    }
}
