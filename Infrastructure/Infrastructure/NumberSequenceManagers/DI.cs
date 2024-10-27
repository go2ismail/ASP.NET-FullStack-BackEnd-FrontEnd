// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.NumberSequenceManagers;

public static class DI
{
    public static IServiceCollection RegisterNumberSequenceManager(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<INumberSequenceService, NumberSequenceService>();

        return services;
    }
}
