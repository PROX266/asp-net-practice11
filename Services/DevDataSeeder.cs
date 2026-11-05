using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Security;

namespace WebApplication1.Services;

public static class DevDataSeeder
{
    public static void Seed(IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DevDataSeeder");
        try
        {
            var context = services.GetRequiredService<WebstoreContext>();
            var hasher = new PasswordHasher<User>();

            var demo = new[]
            {
                (Login: "api_admin",   Password: "Admin123!",   Role: AppRoles.Admin),
                (Login: "api_manager", Password: "Manager123!", Role: AppRoles.Manager),
                (Login: "api_user",    Password: "User123!",    Role: AppRoles.User),
            };

            foreach (var d in demo)
            {
                if (context.Users.Any(u => u.UserLogin == d.Login)) continue;

                var role = context.Roles.FirstOrDefault(r => r.RoleName == d.Role);
                if (role == null)
                {
                    role = new Role { RoleName = d.Role };
                    context.Roles.Add(role);
                    context.SaveChanges();
                }

                var user = new User
                {
                    UserLogin = d.Login,
                    UserName = d.Login,
                    RoleId = role.Id,
                    IsActive = true,
                    UserPassword = Array.Empty<byte>()
                };
                user.PasswordHash = hasher.HashPassword(user, d.Password);
                context.Users.Add(user);
                context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Демо-пользователи не созданы.");
        }
    }
}
