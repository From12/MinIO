using Files.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Files.Application.Interfaces;

public interface IFilesDbContext
{
   
    DbSet<TFile> Files { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}