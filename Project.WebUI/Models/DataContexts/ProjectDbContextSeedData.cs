using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Project.WebUI.Models.Entities.Membership;

namespace Project.WebUI.Models.DataContexts
{
    public static class SokaDbContextSeedData
    {
        public static IApplicationBuilder SeedMembership(this IApplicationBuilder app)
        {
            return app;
            const string adminEmail = "elgunbayramov223@gmail.com";
            const string adminUserName = "elgunbayramov";
            const string adminPassword = "123";
            const string roleName = "admin";

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ProjectDbContext>();
                db.Database.Migrate();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ProjectUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ProjectRole>>();

                var role = roleManager.FindByNameAsync(roleName).Result;

                if (role == null)
                {
                    role = new ProjectRole
                    {
                        Name = roleName
                    };

                    roleManager.CreateAsync(role).Wait();
                }

                var user = userManager.FindByEmailAsync(adminEmail).Result;

                if (user == null)
                {
                    user = new ProjectUser
                    {
                        Email = adminEmail,
                        UserName = adminUserName,
                        EmailConfirmed = true
                    };

                    userManager.CreateAsync(user, adminPassword).Wait();
                }

                if (userManager.IsInRoleAsync(user, roleName).Result == false)
                {
                    userManager.AddToRoleAsync(user, roleName).Wait();
                }
            }

            return app;
        }
    }
}
