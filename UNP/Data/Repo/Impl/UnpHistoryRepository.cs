using Microsoft.EntityFrameworkCore;
using UNP.Data.Repo.Interfaces;
using UNP.Models;

namespace UNP.Data.Repo.Impl
{
    public class UnpHistoryRepository : IUnpHistoryRepository
    {
        private readonly AppDbContext _context;

        public UnpHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UnpHistoryModel>> GetUserHistoriesByEmailAsync(string email)
        {
            return await _context.UnpHistories
                .Where(h => h.Email == email)
                .OrderByDescending(h => h.LastChecked)
                .ToListAsync();
        }
    }
}
