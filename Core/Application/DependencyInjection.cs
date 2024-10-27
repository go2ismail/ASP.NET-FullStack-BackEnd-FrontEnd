// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Common.Behaviors;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        //>>> AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        //>>> FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        //>>> MediatR
        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            x.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            x.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        });

        return services;
    }
}
