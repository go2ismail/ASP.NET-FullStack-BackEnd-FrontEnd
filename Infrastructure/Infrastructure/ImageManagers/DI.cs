// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.ImageManagers;

public static class DI
{
    public static IServiceCollection RegisterImageManager(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ImageManagerSettings>(configuration.GetSection("ImageManager"));
        services.AddTransient<IImageService, ImageService>();

        return services;
    }
}
