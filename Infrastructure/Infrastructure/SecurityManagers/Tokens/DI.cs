// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using System.Text;

namespace Infrastructure.SecurityManagers.Tokens;

public static class DI
{
    public static IServiceCollection RegisterToken(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSectionName = "Jwt";
        services.Configure<TokenSettings>(configuration.GetSection(jwtSectionName));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var tokenSettings = configuration.GetSection(jwtSectionName).Get<TokenSettings>();

            if (tokenSettings?.Key.Length < 32)
            {
                throw new TokenException("JWT key should be minimal 32 character");
            }

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromMinutes(tokenSettings?.ClockSkewInMinute ?? 5),
                ValidIssuer = tokenSettings?.Issuer,
                ValidAudience = tokenSettings?.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings?.Key ?? throw new ArgumentNullException("JWT Key is empty")))
            };

            options.Events = new JwtBearerEvents
            {
                // Prioritizing HttpOnly cookie before checking Authorization header
                OnMessageReceived = context =>
                {
                    var accessToken = context.HttpContext.Request.Cookies["accessToken"];
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                    }
                    else
                    {
                        var authorizationHeader = context.Request.Headers.Authorization.FirstOrDefault();
                        if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
                        {
                            context.Token = authorizationHeader["Bearer ".Length..].Trim();
                        }
                    }

                    return Task.CompletedTask;
                }
            };

        });

        services.AddTransient<ITokenService, TokenService>();
        services.AddScoped<TokenSettings>();

        return services;
    }
}
