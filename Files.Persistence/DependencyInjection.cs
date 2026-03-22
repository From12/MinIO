using Files.Application.Interfaces;
using Files.Domain.Interfaces;
using Files.Persistence.Contexts;
using Files.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Files.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var databaseProvider = configuration.GetValue<string>("DatabaseProvider") ?? "PostgreSQL";

        services.AddDbContext<FilesDbContext>(options =>
        {
            switch (databaseProvider.ToLowerInvariant())
            {
                case "postgresql":
                case "npgsql":
                    options.UseNpgsql(connectionString,
                        optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(FilesDbContext).Assembly.FullName));
                    break;

                case "sqlserver":
                    options.UseSqlServer(connectionString,
                        optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(FilesDbContext).Assembly.FullName));
                    break;

                default:
                    options.UseNpgsql(connectionString,
                        optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(FilesDbContext).Assembly.FullName));
                    break;
            }
        });

        services.AddScoped<IFilesDbContext>(provider =>
            provider.GetRequiredService<FilesDbContext>());

        services.AddScoped<IFileRepository, FileRepository>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FilesDbContext>();

        // Применяем миграции или создаем базу данных
        await context.Database.MigrateAsync();
    }
}