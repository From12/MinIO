using Files.Application.Interfaces;
using Files.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Files.Persistence.Contexts;


public class FilesDbContext : DbContext, IFilesDbContext
{
    public FilesDbContext(DbContextOptions<FilesDbContext> options)
        : base(options)
    {
    }

    public DbSet<TFile> Files { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FilesDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Можно добавить логику для автоматического обновления полей аудита или отправки Domain Events
        return await base.SaveChangesAsync(cancellationToken);
    }
}