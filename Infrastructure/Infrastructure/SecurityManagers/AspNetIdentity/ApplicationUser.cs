// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Microsoft.AspNetCore.Identity;

namespace Infrastructure.SecurityManagers.AspNetIdentity;

public class ApplicationUser : IdentityUser
{

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? ProfilePictureName { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedById { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedById { get; set; }

    public ApplicationUser(
        string email,
        string userName,
        string firstName,
        string lastName,
        string? createdById
        )
    {
        EmailConfirmed = true;
        IsBlocked = false;
        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
        Email = email.Trim();
        UserName = userName.Trim();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        CreatedById = createdById?.Trim();
    }

}
