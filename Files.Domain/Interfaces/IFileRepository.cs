using Files.Domain.Entities;

namespace Files.Domain.Interfaces
{
    public interface IFileRepository
    {
        Task<TFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<TFile?> GetByStoredNameAsync(string storedName, CancellationToken cancellationToken = default);

        Task<IEnumerable<TFile>> GetByUserIdAsync(
            Guid userId,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default);

        Task<int> GetCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<TFile> AddAsync(TFile file, CancellationToken cancellationToken = default);

        Task UpdateAsync(TFile file, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IEnumerable<TFile>> GetExpiredFilesAsync(CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

