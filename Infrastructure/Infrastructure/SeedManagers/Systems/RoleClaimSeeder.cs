// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Domain.Entities;

using Infrastructure.SecurityManagers.Navigations;

using Microsoft.AspNetCore.Identity;

using System.Security.Claims;

namespace Infrastructure.SeedManagers.Systems;

public class RoleClaimSeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;
    public RoleClaimSeeder(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }
    public async Task GenerateDataAsync()
    {
        var adminRole = "Admin";
        if (!await _roleManager.RoleExistsAsync(adminRole))
        {
            await _roleManager.CreateAsync(new IdentityRole(adminRole));
            var role = await _roleManager.FindByNameAsync(adminRole);
            if (role != null)
            {

                foreach (var item in NavigationBuilder
                    .BuildFinalNavigations()
                    .SelectMany(x => x.Children))
                {
                    await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{item.Name}:Create"));
                    await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{item.Name}:Read"));
                    await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{item.Name}:Update"));
                    await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{item.Name}:Delete"));
                }

            }
        }

        var basicRole = "Basic";
        if (!await _roleManager.RoleExistsAsync(basicRole))
        {
            await _roleManager.CreateAsync(new IdentityRole(basicRole));
            var role = await _roleManager.FindByNameAsync(basicRole);
            if (role != null)
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"UserProfile:Create"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"UserProfile:Read"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"UserProfile:Update"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"UserProfile:Delete"));
            }
        }



        var staffRole = "Staff";
        if (!await _roleManager.RoleExistsAsync(staffRole))
        {
            await _roleManager.CreateAsync(new IdentityRole(staffRole));
            var role = await _roleManager.FindByNameAsync(staffRole);
            if (role != null)
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(Customer)}:Create"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(Customer)}:Read"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(Customer)}:Update"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(Customer)}:Delete"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(CustomerContact)}:Create"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(CustomerContact)}:Read"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(CustomerContact)}:Update"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(CustomerContact)}:Delete"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(CustomerGroup)}:Create"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(CustomerGroup)}:Read"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(CustomerGroup)}:Update"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(CustomerGroup)}:Delete"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(CustomerSubGroup)}:Create"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(CustomerSubGroup)}:Read"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(CustomerSubGroup)}:Update"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(CustomerSubGroup)}:Delete"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(Vendor)}:Create"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(Vendor)}:Read"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(Vendor)}:Update"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(Vendor)}:Delete"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(VendorContact)}:Create"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(VendorContact)}:Read"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(VendorContact)}:Update"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(VendorContact)}:Delete"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(VendorGroup)}:Create"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(VendorGroup)}:Read"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(VendorGroup)}:Update"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(VendorGroup)}:Delete"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(VendorSubGroup)}:Create"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(VendorSubGroup)}:Read"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(VendorSubGroup)}:Update"));
                await _roleManager.AddClaimAsync(role, new Claim("Permission", $"{nameof(VendorSubGroup)}:Delete"));
            }
        }

    }
}

