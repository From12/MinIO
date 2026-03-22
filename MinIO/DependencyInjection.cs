using Files.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using MinIOStorage;

namespace Files.Infrastructure;


public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Регистрация MinIO
        services.Configure<MinioOptions>(
            configuration.GetSection(MinioOptions.SectionName));

        services.AddSingleton<IMinioClient>(sp =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<MinioOptions>>().Value;

            var builder = new MinioClient()
                .WithEndpoint(options.Endpoint)
                .WithCredentials(options.AccessKey, options.SecretKey);

            if (options.UseSSL)
            {
                builder.WithSSL();
            }

            return builder.Build();
        });

        // Регистрация сервиса хранения файлов
        services.AddScoped<IFileStorageService, MinioFileStorage>();

        return services;
    }
}