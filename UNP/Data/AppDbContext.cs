using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using UNP.Models;

namespace UNP.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUserModel>(options)
    {
        public DbSet<UnpModel> UnpEntries { get; set; }
        public DbSet<UnpStorageModel> UnpDatas { get; set; }
    }
}
