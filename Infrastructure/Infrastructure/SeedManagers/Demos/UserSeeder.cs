// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Infrastructure.SecurityManagers.AspNetIdentity;

using Microsoft.AspNetCore.Identity;

namespace Infrastructure.SeedManagers.Demos;

public class UserSeeder(
    UserManager<ApplicationUser> userManager)
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task GenerateDataAsync()
    {
        var staffRole = "Staff";
        var basicRole = "Basic";

        var users = new List<(string FirstName, string LastName, string Email, string UserName, string Password)>
            {
                ("Alex", "Taylor", "alex.taylor@example.com", "alextaylor", "1234.Abcd"),
                ("Jordan", "Morgan", "jordan.morgan@example.com", "jordanmorgan", "1234.Abcd"),
                ("Taylor", "Lee", "taylor.lee@example.com", "taylorlee", "1234.Abcd"),
                ("Cameron", "Drew", "cameron.drew@example.com", "camerondrew", "1234.Abcd"),
                ("Casey", "Reese", "casey.reese@example.com", "caseyreese", "1234.Abcd"),
                ("Skyler", "Morgan", "skyler.morgan@example.com", "skylermorgan", "1234.Abcd"),
                ("Avery", "Quinn", "avery.quinn@example.com", "averyquinn", "1234.Abcd"),
                ("Charlie", "Harper", "charlie.harper@example.com", "charlieharper", "1234.Abcd"),
                ("Jamie", "Riley", "jamie.riley@example.com", "jamieriley", "1234.Abcd"),
                ("Riley", "Jordan", "riley.jordan@example.com", "rileyjordan", "1234.Abcd"),
            };

        foreach (var (firstName, lastName, email, userName, password) in users)
        {
            if (await _userManager.FindByEmailAsync(email) == null)
            {
                var applicationUser = new ApplicationUser(
                    email,
                    userName,
                    firstName,
                    lastName,
                    null
                )
                {
                    EmailConfirmed = true
                };

                var createUserResult = await _userManager.CreateAsync(applicationUser, password);

                if (createUserResult == IdentityResult.Success)
                {
                    if (!await _userManager.IsInRoleAsync(applicationUser, staffRole))
                    {
                        await _userManager.AddToRoleAsync(applicationUser, staffRole);
                    }
                    if (!await _userManager.IsInRoleAsync(applicationUser, basicRole))
                    {
                        await _userManager.AddToRoleAsync(applicationUser, basicRole);
                    }
                }
            }
        }
    }
}
