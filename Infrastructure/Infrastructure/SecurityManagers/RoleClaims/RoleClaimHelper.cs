// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Infrastructure.SecurityManagers.AspNetIdentity;
using Infrastructure.SecurityManagers.Navigations;

using Microsoft.AspNetCore.Identity;

using System.Security.Claims;

namespace Infrastructure.SecurityManagers.RoleClaims;

public static class RoleClaimHelper
{
    public static List<string> GetPermissionClaims()
    {
        var claims = new List<string>();
        foreach (var item in NavigationBuilder
            .BuildFinalNavigations()
            .SelectMany(x => x.Children))
        {
            claims.Add($"{item.Name}:Create");
            claims.Add($"{item.Name}:Read");
            claims.Add($"{item.Name}:Update");
            claims.Add($"{item.Name}:Delete");
        }

        return claims;

    }

    public static async Task AddAllClaimsToUser(UserManager<ApplicationUser> userManager, ApplicationUser user)
    {
        foreach (var item in GetPermissionClaims())
        {
            await userManager.AddClaimAsync(user, new Claim("Permission", item));
        }
    }

    public static async Task AddAdminRoleToUser(UserManager<ApplicationUser> userManager, ApplicationUser user)
    {
        var roles = new List<string> { "Admin" };
        foreach (var role in roles)
        {
            if (!await userManager.IsInRoleAsync(user, role))
            {
                var result = await userManager.AddToRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new RoleClaimException($"Error adding role '{role}' to user: {errorMessages}");
                }
            }
        }
    }

    public static async Task AddBasicRoleToUser(UserManager<ApplicationUser> userManager, ApplicationUser user)
    {
        var roles = new List<string> { "Basic" };
        foreach (var role in roles)
        {
            if (!await userManager.IsInRoleAsync(user, role))
            {
                var result = await userManager.AddToRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new RoleClaimException($"Error adding role '{role}' to user: {errorMessages}");
                }
            }
        }
    }

}
