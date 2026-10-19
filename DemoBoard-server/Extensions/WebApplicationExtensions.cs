using Microsoft.AspNetCore.Identity;

namespace DemoBoard_server.Extensions;

public static class WebApplicationExtensions
{
    public static void ConfigureRoles(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var roles = new [] { "Admin", "Person", "Company" };
        foreach (var role in roles)
        {
            var roleExists = roleManager.RoleExistsAsync(role).Result;

            if (!roleExists)
                roleManager.CreateAsync(new IdentityRole(role));
        }
    } 
}