using Files.Application;
using Files.Infrastructure;
using Files.Persistence;
using FilesWebAPI.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/files-api-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Добавление сервисов слоев
builder.Services
    .AddApplication()
    .AddPersistence(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Files API",
        Version = "v1",
        Description = "API для управления файлами с хранением в MinIO"
    });

});

// Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Ограничение размера файла
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        await serviceProvider.InitializeDatabaseAsync();
        Log.Information("База данных успешно инициализирована");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Ошибка при инициализации базы данных");
    }
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Включение Swagger в development и production
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Files API v1");
    options.RoutePrefix = string.Empty; // Swagger UI на корневом пути
});

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

Log.Information("Приложение запущено");

app.Run();

// Для интеграционныъ тестов
public partial class Program { }