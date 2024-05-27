using Microsoft.EntityFrameworkCore;
using UNP.Data.Repo.Interfaces;
using UNP.Models;

namespace UNP.Data.Repo.Impl
{
    public class UnpRepository : IUnpRepository
    {
        private readonly AppDbContext _context;

        public UnpRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UnpStorageModel> GetUnpDataAsync(string unp)
        {
            return await _context.UnpDatas.FirstOrDefaultAsync(u => u.Vunp == unp);
        }

        public async Task AddUnpHistoryAsync(UnpHistoryModel unpHistory)
        {
            await _context.UnpHistories.AddAsync(unpHistory);
            await _context.SaveChangesAsync();
        }
    }
}
