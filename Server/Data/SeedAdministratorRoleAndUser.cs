using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Server.Data;
public static class SeedAdministratorRoleAndUser
{
    internal async static Task Seed(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
        await SeedAdministratorRole(roleManager);
        await SeedAdministratorUser(userManager);
    }

    private async static Task SeedAdministratorRole(RoleManager<IdentityRole> roleManager)
    {
        bool administratorRoleExists = await roleManager.RoleExistsAsync("Administrator");

        if (administratorRoleExists == false)
        {
            var role = new IdentityRole
            {
                Name = "Administrator"
            };

            await roleManager.CreateAsync(role);
        }
    }

    private async static Task SeedAdministratorUser(UserManager<IdentityUser> userManager)
    {
        bool administratorUserExists = await userManager.FindByEmailAsync("admin@example.com") != null;

        if (administratorUserExists == false)
        {
            var administratorUser = new IdentityUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com"
            };

            IdentityResult identityResult = await userManager.CreateAsync(administratorUser, "Password1!");

            if (identityResult.Succeeded)
            {
                await userManager.AddToRoleAsync(administratorUser, "Administrator");
            }
        }
    }
}
