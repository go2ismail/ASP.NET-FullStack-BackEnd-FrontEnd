// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Infrastructure.DataAccessManagers.EFCores.Contexts;
using Infrastructure.SeedManagers.Demos;
using Infrastructure.SeedManagers.Systems;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.SeedManagers;

public static class DI
{
    //>>> System Seed

    public static IServiceCollection RegisterSystemSeedManager(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<RoleClaimSeeder>();
        services.AddScoped<IdentitySeeder>();
        services.AddScoped<CurrencySeeder>();
        services.AddScoped<ConfigSeeder>();
        services.AddScoped<GenderSeeder>();
        services.AddScoped<CustomerGroupSeeder>();
        services.AddScoped<VendorGroupSeeder>();

        return services;
    }


    public static IHost SeedSystemData(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var context = serviceProvider.GetRequiredService<DataContext>();
        if (!context.Config.Any()) //if empty, that's mean never been seeded before
        {
            var roleClaimSeeder = serviceProvider.GetRequiredService<RoleClaimSeeder>();
            roleClaimSeeder.GenerateDataAsync().Wait();

            var identitySeeder = serviceProvider.GetRequiredService<IdentitySeeder>();
            identitySeeder.GenerateDataAsync().Wait();

            var currencySeeder = serviceProvider.GetRequiredService<CurrencySeeder>();
            currencySeeder.GenerateDataAsync().Wait();

            var configSeeder = serviceProvider.GetRequiredService<ConfigSeeder>();
            configSeeder.GenerateDataAsync().Wait();

            var genderSeeder = serviceProvider.GetRequiredService<GenderSeeder>();
            genderSeeder.GenerateDataAsync().Wait();

            var customerGroupSeeder = serviceProvider.GetRequiredService<CustomerGroupSeeder>();
            customerGroupSeeder.GenerateDataAsync().Wait();

            var vendorGroupSeeder = serviceProvider.GetRequiredService<VendorGroupSeeder>();
            vendorGroupSeeder.GenerateDataAsync().Wait();

        }

        return host;
    }

    //>>> Demo Seed
    public static IServiceCollection RegisterDemoSeedManager(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<UserSeeder>();
        services.AddScoped<CustomerSeeder>();
        services.AddScoped<VendorSeeder>();

        return services;
    }

    public static IHost SeedDemoData(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var context = serviceProvider.GetRequiredService<DataContext>();
        if (!context.Customer.Any()) //if empty, that's mean never been seeded before
        {
            var userSeeder = serviceProvider.GetRequiredService<UserSeeder>();
            userSeeder.GenerateDataAsync().Wait();

            var customerSeeder = serviceProvider.GetRequiredService<CustomerSeeder>();
            customerSeeder.GenerateDataAsync().Wait();

            var vendorSeeder = serviceProvider.GetRequiredService<VendorSeeder>();
            vendorSeeder.GenerateDataAsync().Wait();
        }

        return host;
    }
}
