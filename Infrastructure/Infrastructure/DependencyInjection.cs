// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Infrastructure.DataAccessManagers.EFCores;
using Infrastructure.DocumentManagers;
using Infrastructure.EmailManagers;
using Infrastructure.EncryptionManagers;
using Infrastructure.ImageManagers;
using Infrastructure.LoggingManagers.Serilogs;
using Infrastructure.NumberSequenceManagers;
using Infrastructure.SecurityManagers.AspNetIdentity;
using Infrastructure.SecurityManagers.Navigations;
using Infrastructure.SecurityManagers.RoleClaims;
using Infrastructure.SecurityManagers.Tokens;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        //>>> DataAccess
        services.RegisterDataAccess(configuration);

        //>>> AspNetIdentity
        services.RegisterAspNetIdentity(configuration);

        //>>> Policy
        services.RegisterPolicy(configuration);

        //>>> Serilog
        services.RegisterSerilog(configuration);

        //>>> RegisterImageManager
        services.RegisterImageManager(configuration);

        //>>> RegisterDocumentManager
        services.RegisterDocumentManager(configuration);

        //>>> RegisterToken
        services.RegisterToken(configuration);

        //>>> NavigationManager
        services.RegisterNavigationManager(configuration);

        //>>> NumberSequenceManager
        services.RegisterNumberSequenceManager(configuration);

        //>>> EmailManager
        services.RegisterEmailManager(configuration);

        //>>> EncryptionManager
        services.RegisterEncryptionManager(configuration);

        return services;
    }
}
