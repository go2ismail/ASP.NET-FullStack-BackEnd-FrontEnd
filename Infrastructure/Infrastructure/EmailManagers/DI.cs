// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.EmailManagers;

public static class DI
{
    public static IServiceCollection RegisterEmailManager(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IEmailService, EmailService>();

        return services;
    }
}
