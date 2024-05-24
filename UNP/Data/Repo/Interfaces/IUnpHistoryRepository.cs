using UNP.Models;

namespace UNP.Data.Repo.Interfaces
{
    public interface IUnpHistoryRepository
    {
        Task<List<UnpHistoryModel>> GetUserHistoriesByEmailAsync(string email);
    }
}
