using Files.Domain.Entities;
using Files.Domain.Interfaces;
using Files.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Files.Persistence.Repositories;


public class FileRepository : IFileRepository
{
    private readonly FilesDbContext _context;

    public FileRepository(FilesDbContext context)
    {
        _context = context;
    }

    public async Task<TFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Files
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted, cancellationToken);
    }

    public async Task<TFile?> GetByStoredNameAsync(string storedName, CancellationToken cancellationToken = default)
    {
        return await _context.Files
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.StoredName == storedName && !f.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<TFile>> GetByUserIdAsync(
        Guid userId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return await _context.Files
            .AsNoTracking()
            .Where(f => f.UserId == userId && !f.IsDeleted)
            .OrderByDescending(f => f.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Files
            .AsNoTracking()
            .CountAsync(f => f.UserId == userId && !f.IsDeleted, cancellationToken);
    }

    public async Task<TFile> AddAsync(TFile file, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Files.AddAsync(file, cancellationToken);
        return entry.Entity;
    }

    public async Task UpdateAsync(TFile file, CancellationToken cancellationToken = default)
    {
        _context.Files.Update(file);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var file = await _context.Files.FindAsync(new object[] { id }, cancellationToken);
        if (file != null)
        {
            _context.Files.Remove(file);
        }
    }

    public async Task<IEnumerable<TFile>> GetExpiredFilesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Files
            .AsNoTracking()
            .Where(f => f.ExpiresAt.HasValue && f.ExpiresAt.Value < now && !f.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Files
            .AsNoTracking()
            .AnyAsync(f => f.Id == id && !f.IsDeleted, cancellationToken);
    }
}