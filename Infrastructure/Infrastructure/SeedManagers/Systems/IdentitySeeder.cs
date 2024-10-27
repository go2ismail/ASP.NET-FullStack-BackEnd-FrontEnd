// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Infrastructure.SecurityManagers.AspNetIdentity;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Infrastructure.SeedManagers.Systems;

public class IdentitySeeder(
    IOptions<IdentitySettings> identitySettings,
    UserManager<ApplicationUser> userManager)
{
    private readonly IdentitySettings _identitySettings = identitySettings.Value;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task GenerateDataAsync()
    {
        var adminEmail = _identitySettings.DefaultAdmin.Email;
        var adminPassword = _identitySettings.DefaultAdmin.Password;

        var adminRole = "Admin";
        var basicRole = "Basic";
        if (await _userManager.FindByEmailAsync(adminEmail) == null)
        {
            var applicationUser = new ApplicationUser(
                adminEmail,
                adminEmail.Split('@')[0].Replace(".", string.Empty),
                "Root",
                "Admin",
                null
                )
            {
                EmailConfirmed = true
            };

            //create user Root Admin
            var createUserResult = await _userManager.CreateAsync(applicationUser, adminPassword);

            if (createUserResult == IdentityResult.Success)
            {
                //add Admin role to Root Admin
                if (!await _userManager.IsInRoleAsync(applicationUser, adminRole))
                {
                    await _userManager.AddToRoleAsync(applicationUser, adminRole);
                }

                //add Basic role to Root Admin
                if (!await _userManager.IsInRoleAsync(applicationUser, basicRole))
                {
                    await _userManager.AddToRoleAsync(applicationUser, basicRole);
                }
            }
        }
    }
}
