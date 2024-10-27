// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DocumentManagers;

public static class DI
{
    public static IServiceCollection RegisterDocumentManager(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DocumentManagerSettings>(configuration.GetSection("DocumentManager"));
        services.AddTransient<IDocumentService, DocumentService>();

        return services;
    }
}
