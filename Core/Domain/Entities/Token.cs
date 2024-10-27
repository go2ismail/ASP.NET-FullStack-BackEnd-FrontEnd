// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Domain.Bases;
using Domain.Constants;
using Domain.Interfaces;

namespace Domain.Entities;

public class Token : BaseEntity, IAggregateRoot
{

    public string UserId { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTimeOffset ExpiryDate { get; set; }

    public Token() : base() { } //for EF Core
    public Token(
        string userId,
        string refreshToken
    )
    {
        UserId = userId.Trim();
        RefreshToken = refreshToken.Trim();
        ExpiryDate = DateTime.UtcNow.AddDays(TokenConsts.ExpiryInDays);
    }

}
