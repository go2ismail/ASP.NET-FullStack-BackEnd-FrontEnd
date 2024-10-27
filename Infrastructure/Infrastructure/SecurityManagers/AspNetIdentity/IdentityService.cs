// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Common.Models;
using Application.Features.Accounts.Commands;
using Application.Features.Accounts.Dtos;
using Application.Features.Accounts.Queries;
using Application.Features.Members.Commands;
using Application.Services.Externals;
using Application.Services.Repositories;

using Domain.Entities;

using Infrastructure.DataAccessManagers.EFCores.Contexts;
using Infrastructure.SecurityManagers.RoleClaims;
using Infrastructure.SecurityManagers.Tokens;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

using System.Security.Claims;
using System.Text;

namespace Infrastructure.SecurityManagers.AspNetIdentity;

public class IdentityService(
    IOptions<IdentitySettings> identitySettings,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
     SignInManager<ApplicationUser> signInManager,
    IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
    IAuthorizationService authorizationService,
    ITokenService tokenService,
    ITokenRepository tokenRepository,
    IUnitOfWork unitOfWork,
    INavigationService navigationService,
    IRoleClaimService roleClaimService,
    QueryContext queryContext
        ) : IIdentityService
{

    private readonly IdentitySettings _identitySettings = identitySettings.Value;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService = authorizationService;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ITokenRepository _tokenRepository = tokenRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly INavigationService _navigationService = navigationService;
    private readonly IRoleClaimService _roleClaimService = roleClaimService;
    private readonly QueryContext _queryContext = queryContext;

    public async Task<GetClaimsByUserResult> GetClaimsByUserAsync(
        string userId,
        int page = 1,
        int limit = 10,
        string sortBy = "Value",
        string sortDirection = "asc",
        string searchValue = "",
        CancellationToken cancellationToken = default)
    {

        var claims = await _roleClaimService.GetClaimListByUserAsync(userId, cancellationToken);
        var userClaims = claims.Select(x => new ClaimDto { Type = x.Type, Value = x.Value }).ToList();

        if (
            searchValue is not null
            && searchValue != "null"
            && !string.IsNullOrWhiteSpace(searchValue.Trim())
            )
        {
            userClaims = userClaims
                .Where(x => x.Type != null && x.Type.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                             x.Value != null && x.Value.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (sortBy.Equals("Value", StringComparison.OrdinalIgnoreCase))
        {
            userClaims = sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? [.. userClaims.OrderBy(x => x.Value)]
                : [.. userClaims.OrderByDescending(x => x.Value)];
        }

        return new GetClaimsByUserResult { Claims = PagedList<ClaimDto>.FromList(userClaims, page, limit) };
    }


    public async Task<GetClaimsResult> GetClaimsAsync(
        int page = 1,
        int limit = 10,
        string sortBy = "Value",
        string sortDirection = "asc",
        string searchValue = "",
        CancellationToken cancellationToken = default)
    {

        var claims = await _roleClaimService.GetClaimListAsync(cancellationToken);

        int counter = 1;
        var userClaims = claims.Select(x => new ClaimDto
        {
            Id = counter++.ToString(),
            Type = x.Type,
            Value = x.Value
        }).ToList();

        if (
            searchValue is not null
            && searchValue != "null"
            && !string.IsNullOrWhiteSpace(searchValue.Trim())
            )
        {
            userClaims = userClaims
                .Where(x => x.Type != null && x.Type.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                             x.Value != null && x.Value.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }


        if (sortBy.Equals("Value", StringComparison.OrdinalIgnoreCase))
        {
            userClaims = sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? [.. userClaims.OrderBy(x => x.Value)]
                : [.. userClaims.OrderByDescending(x => x.Value)];
        }

        return new GetClaimsResult { Data = PagedList<ClaimDto>.FromList(userClaims, page, limit) };
    }


    public async Task<GetClaimsByRoleResult> GetClaimsByRoleAsync(
        string role,
        int page = 1,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var roleIdentity = await _roleManager.FindByNameAsync(role) ?? throw new IdentityException($"Role not found. Role: {role}");
        var roleClaims = new List<string>();
        var claimsForRole = await _roleManager.GetClaimsAsync(roleIdentity);
        roleClaims.AddRange(claimsForRole.Select(x => x.Value).ToList());

        cancellationToken.ThrowIfCancellationRequested();

        return new GetClaimsByRoleResult { Claims = PagedList<string>.FromList(roleClaims, page, limit) };
    }

    public async Task<List<LookupDto>> GetClaimLookupAsync(CancellationToken cancellationToken = default)
    {

        var claims = await _roleClaimService.GetClaimListAsync(cancellationToken);

        var lookupClaims = claims.Select(x => new LookupDto
        {
            Text = x.Value,
            Value = x.Value
        }).ToList();

        return lookupClaims;
    }

    public async Task<GetRolesResult> GetRolesAsync(
        int page = 1,
        int limit = 10,
        string sortBy = "Name",
        string sortDirection = "asc",
        string searchValue = "",
        CancellationToken cancellationToken = default)
    {

        cancellationToken.ThrowIfCancellationRequested();

        int counter = 1;
        var roles = _roleManager.Roles
                                .Select(r => r.Name)
                                .Where(name => name != null)
                                .ToList()
                                .Select(name => new RoleDto
                                {
                                    Id = counter++.ToString(),
                                    Name = name
                                })
                                .ToList();

        if (
            searchValue is not null
            && searchValue != "null"
            && !string.IsNullOrWhiteSpace(searchValue.Trim())
            )
        {
            roles = roles
                .Where(x => x.Name != null && x.Name.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
        {
            roles = sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? [.. roles.OrderBy(x => x.Name)]
                : [.. roles.OrderByDescending(x => x.Name)];
        }

        foreach (var role in roles)
        {
            if (role != null && role.Name != null)
            {
                var identityRole = await _roleManager.FindByNameAsync(role.Name);
                if (identityRole != null)
                {
                    var claims = await _roleManager.GetClaimsAsync(identityRole);
                    role.Claims = claims.Select(c => c.Value).ToList();
                }
            }
        }

        await Task.CompletedTask;

        cancellationToken.ThrowIfCancellationRequested();

        return new GetRolesResult { Data = PagedList<RoleDto>.FromList(roles, page, limit) };
    }

    public async Task<GetRoleLookupResult> GetRoleLookupAsync(
        CancellationToken cancellationToken = default)
    {

        cancellationToken.ThrowIfCancellationRequested();

        var roles = _roleManager.Roles
                                .Select(x => new LookupDto
                                {
                                    Text = x.Name ?? "",
                                    Value = x.Name ?? ""
                                })
                                .ToList();

        await Task.CompletedTask;

        cancellationToken.ThrowIfCancellationRequested();

        return new GetRoleLookupResult { Data = roles, Message = "Success" };
    }

    public async Task<GetRolesByUserResult> GetRolesByUserAsync(
        string userId,
        int page = 1,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByIdAsync(userId) ?? throw new IdentityException($"User not found. ID: {userId}");
        List<Claim> userClaims = [.. (await _userManager.GetClaimsAsync(user))];

        var roles = await _userManager.GetRolesAsync(user);

        cancellationToken.ThrowIfCancellationRequested();

        return new GetRolesByUserResult { Roles = PagedList<string>.FromList([.. roles], page, limit) };
    }

    public async Task<string?> GetUserNameAsync(string userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByIdAsync(userId);

        cancellationToken.ThrowIfCancellationRequested();

        return user?.UserName;
    }

    public async Task<CreateUserResult> CreateUserAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        string createdById,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = new ApplicationUser(
            email,
            email.Split('@')[0].Replace(".", string.Empty),
            firstName,
            lastName,
            createdById
        );

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            throw new IdentityException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await RoleClaimHelper.AddBasicRoleToUser(_userManager, user);

        cancellationToken.ThrowIfCancellationRequested();

        return new CreateUserResult
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }

    public async Task<RegisterUserResult> RegisterUserAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = new ApplicationUser(
            email,
            email.Split('@')[0].Replace(".", string.Empty),
            firstName,
            lastName,
            null
        );

        var sendEmailConfirmation = false;
        if (_identitySettings.SignIn.RequireConfirmedEmail)
        {
            //email confirmation should be sent to confirmed the registered email.
            user.EmailConfirmed = false;
            sendEmailConfirmation = true;
        }
        else
        {
            user.EmailConfirmed = true;
        }
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            throw new IdentityException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        await RoleClaimHelper.AddBasicRoleToUser(_userManager, user);

        cancellationToken.ThrowIfCancellationRequested();

        return new RegisterUserResult
        {
            Id = user.Id,
            Email = email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailConfirmationToken = code,
            SendEmailConfirmation = sendEmailConfirmation,
        };
    }

    public async Task<CreateMemberResult> CreateMemberAsync(
            string email,
            string password,
            string firstName,
            string lastName,
            bool emailConfirmed,
            bool isBlocked,
            string[]? roles,
            CancellationToken cancellationToken = default
        )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = new ApplicationUser(
            email,
            email.Split('@')[0].Replace(".", string.Empty),
            firstName,
            lastName,
            null
        )
        {
            EmailConfirmed = emailConfirmed,
            IsBlocked = isBlocked
        };
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            throw new IdentityException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        if (roles != null)
        {

            foreach (var role in roles)
            {
                await _userManager.AddToRoleAsync(user, role);
            }

        }

        cancellationToken.ThrowIfCancellationRequested();

        return new CreateMemberResult
        {
            Id = user.Id,
            Email = email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }


    public async Task<UpdateMemberResult> UpdateMemberAsync(
        string email,
        string? password,
        string firstName,
        string lastName,
        bool emailConfirmed,
        bool isBlocked,
        bool isDeleted,
        string[]? roles,
        CancellationToken cancellationToken = default
        )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByEmailAsync(email) ?? throw new Exception($"User not found. {email}");
        if (!string.IsNullOrEmpty(password))
        {
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
            {
                throw new Exception("Failed to remove the old password.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, password);
            if (!addPasswordResult.Succeeded)
            {
                throw new Exception("Failed to set the new password.");
            }
        }

        user.FirstName = firstName;
        user.LastName = lastName;
        user.EmailConfirmed = emailConfirmed;
        user.IsBlocked = isBlocked;
        user.IsDeleted = isDeleted;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);

        var existingRoles = await _userManager.GetRolesAsync(user);

        foreach (var role in existingRoles)
        {
            await _userManager.RemoveFromRoleAsync(user, role);
        }

        if (roles != null)
        {

            foreach (var role in roles)
            {
                await _userManager.AddToRoleAsync(user, role);
            }

        }

        if (!result.Succeeded)
        {
            throw new IdentityException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }


        cancellationToken.ThrowIfCancellationRequested();

        return new UpdateMemberResult
        {
            Id = user.Id,
            Email = email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }


    public async Task<string> UpdateProfilePictureAsync(
        string email,
        string profilePictureName,
        CancellationToken cancellationToken = default
        )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByEmailAsync(email) ?? throw new Exception($"User not found. {email}");
        user.ProfilePictureName = profilePictureName;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new IdentityException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }


        cancellationToken.ThrowIfCancellationRequested();

        return profilePictureName;
    }

    public async Task<DeleteMemberResult> DeleteMemberAsync(
    string email,
    CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByEmailAsync(email) ?? throw new Exception($"User not found. {email}");
        user.IsDeleted = true;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new IdentityException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }


        cancellationToken.ThrowIfCancellationRequested();

        return new DeleteMemberResult
        {
            Id = user.Id,
            Email = email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }


    public async Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByIdAsync(userId) ?? throw new IdentityException($"User not found. ID: {userId}");
        var result = await _userManager.IsInRoleAsync(user, role);

        cancellationToken.ThrowIfCancellationRequested();

        return result;
    }
    public async Task<AddRolesToUserResult> AddRolesToUserAsync(string userId, string[] roles, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByIdAsync(userId) ?? throw new IdentityException($"User not found. ID: {userId}");
        foreach (var role in roles)
        {
            if (!await _userManager.IsInRoleAsync(user, role))
            {
                var result = await _userManager.AddToRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new IdentityException($"Error adding role '{role}' to user: {errorMessages}");
                }

            }
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        cancellationToken.ThrowIfCancellationRequested();

        return new AddRolesToUserResult
        {
            UserId = userId,
            Roles = [.. userRoles]
        };
    }
    public async Task<DeleteRolesFromUserResult> DeleteRolesFromUserAsync(string userId, string[] roles, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByIdAsync(userId) ?? throw new IdentityException($"User not found. ID: {userId}");
        foreach (var role in roles)
        {
            if (await _userManager.IsInRoleAsync(user, role))
            {
                var result = await _userManager.RemoveFromRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new IdentityException($"Error deleting role '{role}' from user: {errorMessages}");
                }

            }
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        cancellationToken.ThrowIfCancellationRequested();

        return new DeleteRolesFromUserResult
        {
            UserId = userId,
            Roles = [.. userRoles]
        };
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        cancellationToken.ThrowIfCancellationRequested();

        return result.Succeeded;
    }

    public async Task<DeleteUserResult> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByIdAsync(userId) ?? throw new IdentityException($"User not found. ID: {userId}");
        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            throw new IdentityException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        cancellationToken.ThrowIfCancellationRequested();

        return new DeleteUserResult
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }

    public async Task<LoginUserResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email) ?? throw new UnauthorizedAccessException("Invalid login credentials.");
        if (user.IsBlocked)
        {
            throw new UnauthorizedAccessException($"User is blocked. {email}");
        }

        if (user.IsDeleted)
        {
            throw new UnauthorizedAccessException($"User already deleted. {email}");
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, true, lockoutOnFailure: false);

        if (result.IsLockedOut)
        {
            throw new UnauthorizedAccessException("Invalid login credentials. IsLockedOut.");
        }

        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid login credentials. NotSucceeded.");
        }


        var mainNavs = await _navigationService.GenerateMainNavAsync(user.Id, cancellationToken);
        var userClaims = await _roleClaimService.GetClaimListByUserAsync(user.Id);
        var accessToken = _tokenService.GenerateToken(user, userClaims);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var tokens = await _tokenRepository.GetByUserIdAsync(user.Id, cancellationToken);
        foreach (var tokenItem in tokens)
        {
            _tokenRepository.Purge(tokenItem);
        }

        var token = new Token(user.Id, refreshToken);
        await _tokenRepository.CreateAsync(token, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new LoginUserResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserClaims = userClaims.Select(x => x.Value).ToList(),
            MainNavigations = mainNavs.MainNavigations
        };
    }

    public async Task<LogoutUserResult> LogoutAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId) ?? throw new IdentityException($"Invalid userId: {userId}");
        var tokens = await _tokenRepository.GetByUserIdAsync(userId, cancellationToken);
        foreach (var token in tokens)
        {
            _tokenRepository.Purge(token);
        }

        await _unitOfWork.SaveAsync(cancellationToken);

        return new LogoutUserResult { UserId = user.Id };
    }

    public async Task<GenerateRefreshTokenResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {

        var token = await _tokenRepository.GetByRefreshTokenAsync(refreshToken, cancellationToken);

        if (token.ExpiryDate < DateTime.UtcNow)
        {
            throw new IdentityException("Refresh token has expired, please re-login");
        }

        var user = await _userManager.FindByIdAsync(token.UserId) ?? throw new IdentityException("Refresh token has expired, please re-login");
        var mainNavs = await _navigationService.GenerateMainNavAsync(user.Id, cancellationToken);
        var userClaims = await _roleClaimService.GetClaimListByUserAsync(user.Id);
        var newAccessToken = _tokenService.GenerateToken(user, userClaims);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        _tokenRepository.Purge(token);

        var newToken = new Token(user.Id, newRefreshToken);
        await _tokenRepository.CreateAsync(newToken, cancellationToken);

        await _unitOfWork.SaveAsync(cancellationToken);

        return new GenerateRefreshTokenResult
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserClaims = userClaims.Select(x => x.Value).ToList(),
            MainNavigations = mainNavs.MainNavigations
        };
    }

    public async Task<AddClaimsToRoleResult> AddClaimsToRoleAsync(string role, string[] claims, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var identityRole = await _roleManager.FindByNameAsync(role) ?? throw new IdentityException($"Role '{role}' not found.");
        if (!RoleClaimHelper.GetPermissionClaims().Any(x => claims.Contains(x)))
        {
            throw new IdentityException($"Contain not valid claims: {string.Join("; ", claims.Select(x => x))}.");
        }

        foreach (var claim in claims)
        {
            var claimToAdd = new Claim("Permission", claim);
            var result = await _roleManager.AddClaimAsync(identityRole, claimToAdd);

            if (!result.Succeeded)
            {
                throw new IdentityException($"Failed to add claim '{claim}' to role '{role}'.");
            }
        }

        cancellationToken.ThrowIfCancellationRequested();

        return new AddClaimsToRoleResult
        {
            Role = role,
            Claims = claims
        };
    }

    public async Task<DeleteClaimsFromRoleResult> DeleteClaimsFromRoleAsync(string role, string[] claims, CancellationToken cancellationToken = default)
    {

        cancellationToken.ThrowIfCancellationRequested();

        var identityRole = await _roleManager.FindByNameAsync(role) ?? throw new IdentityException($"Role '{role}' not found.");
        if (!RoleClaimHelper.GetPermissionClaims().Any(x => claims.Contains(x)))
        {
            throw new IdentityException($"Contain not valid claims: {string.Join("; ", claims.Select(x => x))}.");
        }

        foreach (var claim in claims)
        {
            var claimToDelete = new Claim("Permission", claim);
            var result = await _roleManager.RemoveClaimAsync(identityRole, claimToDelete);

            if (!result.Succeeded)
            {
                throw new IdentityException($"Failed to delete claim '{claim}' from role '{role}'.");
            }
        }

        cancellationToken.ThrowIfCancellationRequested();

        return new DeleteClaimsFromRoleResult
        {
            Role = role,
            Claims = claims
        };
    }

    public async Task<CreateRoleResult> CreateRoleAsync(
        string role,
        string[] claims,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!await _roleManager.RoleExistsAsync(role))
        {
            var newRole = new IdentityRole(role);
            var identityResult = await _roleManager.CreateAsync(newRole);
            if (!identityResult.Succeeded)
            {
                throw new IdentityException($"CreateRolesAsync error: {identityResult.Errors.Select(e => e.Description)}");
            }

            foreach (var claim in claims)
            {
                var addResult = await _roleManager.AddClaimAsync(newRole, new Claim("Permission", claim));
                if (!addResult.Succeeded)
                {
                    throw new IdentityException("Error adding new claims to the role.");
                }
            }

        }
        else
        {
            throw new IdentityException($"Role {role} already exists.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        return new CreateRoleResult { Role = role, Claims = claims };
    }

    public async Task<DeleteRoleResult> DeleteRoleAsync(
        string role,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var deletedRole = await _roleManager.FindByNameAsync(role);
        if (deletedRole != null)
        {
            // Remove all claims associated with the role
            var claims = await _roleManager.GetClaimsAsync(deletedRole);
            foreach (var claim in claims)
            {
                var removeClaimResult = await _roleManager.RemoveClaimAsync(deletedRole, claim);
                if (!removeClaimResult.Succeeded)
                {
                    throw new IdentityException("Error removing claims from the role.");
                }
            }

            // Delete the role
            var result = await _roleManager.DeleteAsync(deletedRole);
            if (!result.Succeeded)
            {
                throw new IdentityException($"Error deleting role {role}.");
            }
        }
        else
        {
            throw new IdentityException($"Role {role} not found.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        return new DeleteRoleResult { };
    }


    public async Task<UpdateRoleResult> UpdateRoleAsync(
        string oldRole,
        string newRole,
        string[] newClaims,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Find the role by its old name
        var role = await _roleManager.FindByNameAsync(oldRole)
            ?? throw new IdentityException($"Role {oldRole} not found.");

        // If the role name is changing, update it
        if (oldRole != newRole)
        {
            role.Name = newRole;
            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                throw new IdentityException($"Error: Update role name from {oldRole} to {newRole}");
            }
        }

        // Remove existing claims
        var existingClaims = await _roleManager.GetClaimsAsync(role);
        foreach (var claim in existingClaims)
        {
            var removeResult = await _roleManager.RemoveClaimAsync(role, claim);
            if (!removeResult.Succeeded)
            {
                throw new IdentityException("Error removing existing claims from the role.");
            }
        }

        // Add new claims
        foreach (var claim in newClaims)
        {
            var addResult = await _roleManager.AddClaimAsync(role, new Claim("Permission", claim));
            if (!addResult.Succeeded)
            {
                throw new IdentityException("Error adding new claims to the role.");
            }
        }

        cancellationToken.ThrowIfCancellationRequested();

        return new UpdateRoleResult { Role = newRole, Claims = newClaims };
    }




    public async Task<GetUsersResult> GetUsersAsync(
        int page = 1,
        int limit = 10,
        string sortBy = "Email",
        string sortDirection = "asc",
        string searchValue = "",
        CancellationToken cancellationToken = default)
    {

        cancellationToken.ThrowIfCancellationRequested();

        var users = _userManager.Users
            .Select(user => new ApplicationUserDto
            {
                Id = user.Id,
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfilePictureName = user.ProfilePictureName,
                EmailConfirmed = user.EmailConfirmed,
                IsBlocked = user.IsBlocked,
                IsDeleted = user.IsDeleted,
                Roles = new List<string>(),
                Claims = new List<string>()
            })
            .ToList();

        foreach (var user in users)
        {
            if (user != null && user.UserId != null)
            {
                var userFromDb = await _userManager.FindByIdAsync(user.UserId);
                if (userFromDb != null)
                {
                    user.Roles = await _userManager.GetRolesAsync(userFromDb);
                    var claims = await _roleClaimService.GetClaimListByUserAsync(userFromDb.Id, cancellationToken);
                    user.Claims = claims.Select(c => c.Value).ToList();
                }
            }
        }


        if (
            searchValue is not null
            && searchValue != "null"
            && !string.IsNullOrWhiteSpace(searchValue.Trim())
            )
        {
            users = users
                .Where(x =>
                    x.FirstName != null && x.FirstName.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                    x.LastName != null && x.LastName.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                    x.Email != null && x.Email.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (sortBy.Equals("Email", StringComparison.OrdinalIgnoreCase))
        {
            users = sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? [.. users.OrderBy(x => x.Email)]
                : [.. users.OrderByDescending(x => x.Email)];
        }

        if (sortBy.Equals("FirstName", StringComparison.OrdinalIgnoreCase))
        {
            users = sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? [.. users.OrderBy(x => x.FirstName)]
                : [.. users.OrderByDescending(x => x.FirstName)];
        }

        if (sortBy.Equals("LastName", StringComparison.OrdinalIgnoreCase))
        {
            users = sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? [.. users.OrderBy(x => x.LastName)]
                : [.. users.OrderByDescending(x => x.LastName)];
        }

        await Task.CompletedTask;

        cancellationToken.ThrowIfCancellationRequested();

        return new GetUsersResult { Data = PagedList<ApplicationUserDto>.FromList(users, page, limit) };
    }

    public async Task<GetUsersByUserIdResult> GetUsersByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {

        cancellationToken.ThrowIfCancellationRequested();

        if (userId == null)
        {
            throw new IdentityException($"User must not null");
        }

        var users = _userManager.Users
            .Where(x => x.Id == userId)
            .Select(user => new ApplicationUserDto
            {
                Id = user.Id,
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfilePictureName = user.ProfilePictureName,
                EmailConfirmed = user.EmailConfirmed,
                IsBlocked = user.IsBlocked,
                IsDeleted = user.IsDeleted,
                Roles = new List<string>(),
                Claims = new List<string>()
            })
            .ToList();

        foreach (var user in users)
        {
            if (user != null && user.UserId != null)
            {
                var userFromDb = await _userManager.FindByIdAsync(user.UserId);
                if (userFromDb != null)
                {
                    user.Roles = await _userManager.GetRolesAsync(userFromDb);
                    var claims = await _roleClaimService.GetClaimListByUserAsync(userFromDb.Id, cancellationToken);
                    user.Claims = claims.Select(c => c.Value).ToList();
                }
            }
        }



        await Task.CompletedTask;

        cancellationToken.ThrowIfCancellationRequested();

        return new GetUsersByUserIdResult { Users = users };
    }

    public async Task<GetUserByUserIdResult> GetUserByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {

        cancellationToken.ThrowIfCancellationRequested();

        if (userId == null)
        {
            throw new IdentityException($"User must not null");
        }

        var user = _userManager.Users
            .Where(x => x.Id == userId)
            .Select(user => new ApplicationUserDto
            {
                Id = user.Id,
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfilePictureName = user.ProfilePictureName,
                EmailConfirmed = user.EmailConfirmed,
                IsBlocked = user.IsBlocked,
                IsDeleted = user.IsDeleted,
                Roles = new List<string>(),
                Claims = new List<string>()
            })
            .FirstOrDefault();

        await Task.CompletedTask;

        cancellationToken.ThrowIfCancellationRequested();

        return new GetUserByUserIdResult { User = user };
    }

    public List<string> GetUserIdsByUserName(string userName)
    {
        return [.. _queryContext.Users
            .Where(x => x.UserName!.Contains(userName))
            .Select(x => x.Id)];
    }

    public Dictionary<string, string?> GetUsersDictionary()
    {
        return _queryContext.Users.ToDictionary(x => x.Id, x => x.UserName);
    }

    public async Task<string> ConfirmEmailAsync(
        string email,
        string code,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email) ?? throw new IdentityException($"Unable to load user with email: {email}");
        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (!result.Succeeded)
        {
            throw new IdentityException($"Error confirming your email: {email}");
        }

        return email;
    }

    public async Task<ForgotPasswordResult> ForgotPasswordAsync(
    string email,
    CancellationToken cancellationToken = default
)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByEmailAsync(email) ?? throw new IdentityException($"Unable to load user with email: {email}");
        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var clearTempPassword = Guid.NewGuid().ToString()[.._identitySettings.Password.RequiredLength];
        var tempPassword = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(clearTempPassword));

        cancellationToken.ThrowIfCancellationRequested();

        return new ForgotPasswordResult
        {
            Email = email,
            TempPassword = tempPassword,
            EmailConfirmationToken = code,
            ClearTempPassword = clearTempPassword
        };
    }

    public async Task<string> ForgotPasswordConfirmationAsync(
        string email,
        string tempPassword,
        string code,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email) ?? throw new IdentityException($"Unable to load user with email: {email}");
        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        tempPassword = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(tempPassword));
        var result = await _userManager.ResetPasswordAsync(user, code, tempPassword);

        if (!result.Succeeded)
        {
            throw new IdentityException($"Error resetting your password");
        }

        return email;
    }

}
