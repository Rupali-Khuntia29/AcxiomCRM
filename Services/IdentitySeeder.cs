using AcxiomCRM.Models;
using Microsoft.AspNetCore.Identity;

namespace AcxiomCRM.Services;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roles = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { "Admin", "Manager", "SalesExecutive" })
            if (!await roles.RoleExistsAsync(role))
                await roles.CreateAsync(new IdentityRole(role));

        // Demo admin is created only when explicitly configured in environment variables.
        var email = Environment.GetEnvironmentVariable("ACXIOM_ADMIN_EMAIL");
        var password = Environment.GetEnvironmentVariable("ACXIOM_ADMIN_PASSWORD");
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;

        var users = services.GetRequiredService<UserManager<ApplicationUser>>();
        var admin = await users.FindByEmailAsync(email);
        if (admin == null)
        {
            admin = new ApplicationUser { UserName = email, Email = email, FullName = "CRM Administrator", EmailConfirmed = true };
            var result = await users.CreateAsync(admin, password);
            if (result.Succeeded) await users.AddToRoleAsync(admin, "Admin");
        }
    }
}
