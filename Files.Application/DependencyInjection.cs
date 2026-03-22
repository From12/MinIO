using System.Reflection;
using Files.Application.Common.Mappings;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;



namespace Files.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
        });

        services.AddAutoMapper(cfg => { }, assembly);

        services.AddValidatorsFromAssembly(assembly);

        // Регистрация валидаторов для pipeline behavior
        services.AddTransient(typeof(MediatR.IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        return services;
    }
}
