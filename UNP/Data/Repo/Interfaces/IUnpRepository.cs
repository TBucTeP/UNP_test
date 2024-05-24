using UNP.Models;

namespace UNP.Data.Repo.Interfaces
{
    public interface IUnpRepository
    {
        Task<UnpStorageModel> GetUnpDataAsync(string unp);
        Task AddUnpHistoryAsync(UnpHistoryModel unpHistory);
        Task SaveChangesAsync();

    }
}
