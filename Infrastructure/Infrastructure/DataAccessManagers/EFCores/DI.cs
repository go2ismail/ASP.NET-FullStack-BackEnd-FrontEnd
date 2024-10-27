// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.CQS.Commands;
using Application.Services.CQS.Queries;
using Application.Services.Repositories;

using Domain.Invariants;

using Infrastructure.DataAccessManagers.EFCores.Contexts;
using Infrastructure.DataAccessManagers.EFCores.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;

using System.Reflection;

namespace Infrastructure.DataAccessManagers.EFCores;

public static class DI
{
    public static IServiceCollection RegisterDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        connectionString = Check.IsNotNullOrWhiteSpace(connectionString, nameof(connectionString));
        var databaseProvider = configuration["DatabaseProvider"];

        // Register Context
        switch (databaseProvider)
        {
            case "PostgreSql":
                services.AddDbContext<DataContext>(options =>
                    options.UseNpgsql(connectionString)
                    .LogTo(Log.Information, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                );
                services.AddDbContext<CommandContext>(options =>
                    options.UseNpgsql(connectionString)
                    .LogTo(Log.Information, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                );
                services.AddDbContext<QueryContext>(options =>
                    options.UseNpgsql(connectionString)
                    .LogTo(Log.Information, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                );
                break;

            case "MySql":
                services.AddDbContext<DataContext>(options =>
                    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21)))
                    .LogTo(Log.Information, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                );
                services.AddDbContext<CommandContext>(options =>
                    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21)))
                    .LogTo(Log.Information, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                );
                services.AddDbContext<QueryContext>(options =>
                    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21)))
                    .LogTo(Log.Information, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                );
                break;

            case "SqlServer":
            default:
                services.AddDbContext<DataContext>(options =>
                    options.UseSqlServer(connectionString)
                    .LogTo(Log.Information, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                );
                services.AddDbContext<CommandContext>(options =>
                    options.UseSqlServer(connectionString)
                    .LogTo(Log.Information, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                );
                services.AddDbContext<QueryContext>(options =>
                    options.UseSqlServer(connectionString)
                    .LogTo(Log.Information, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                );
                break;
        }

        services.AddScoped<ICommandContext, CommandContext>();
        services.AddScoped<IQueryContext, QueryContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IBaseCommandRepository<>), typeof(BaseCommandRepository<>));

        // Register all repositories dynamically
        var repositoryAssembly = Assembly.GetExecutingAssembly();
        var types = repositoryAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Repository"))
            .ToList();

        foreach (var implementationType in types)
        {
            var interfaces = implementationType.GetInterfaces()
                .Where(i => i.Name.EndsWith("Repository") && i.IsGenericType == false)
                .ToList();

            foreach (var interfaceType in interfaces)
            {
                services.AddScoped(interfaceType, implementationType);
            }
        }

        return services;
    }

    public static IHost CreateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        // Create database using DataContext
        var dataContext = serviceProvider.GetRequiredService<DataContext>();
        dataContext.Database.EnsureDeletedAsync().Wait();
        dataContext.Database.EnsureCreated(); // Ensure database is created (development only)
        //dataContext.Database.Migrate(); // Ensure database is created via migration (production friendly)

        return host;
    }
}
