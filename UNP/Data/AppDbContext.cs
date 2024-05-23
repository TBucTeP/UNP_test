using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using UNP.Models;

namespace UNP.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUserModel>
    {
        public DbSet<UnpHistoryModel> UnpHistories { get; set; }
        public DbSet<UnpStorageModel> UnpDatas { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public async Task InitializeAsync()
        {
            if (!UnpDatas.Any())
            {
                await UnpDatas.AddRangeAsync(
                    new UnpStorageModel { Vunp = "123456789", Vnaimp = "Министерство по налогам и сборам Республики Беларусь", Vnaimk = "МНС", Vpadres = "г.Минск,ул.Советская,9", Dreg = "1994-06-30", Nmns = "104", Vmns = "Инспекция МНС по Московскому району г.Минска", Ckodsost = "1", Dlikv = "null", Vlikv = "null", LastChecked = DateTime.Now },
                    new UnpStorageModel { Vunp = "1234567890", Vnaimp = "ОАО БелГАЗ", Vnaimk = "БелГаз", Vpadres = "г.Минск,ул.Бабушкина,10", Dreg = "1995-07-15", Nmns = "102", Vmns = "Инспекция МНС по Центральному району г.Минска", Ckodsost = "1", Dlikv = "null", Vlikv = "null", LastChecked = DateTime.Now },
                    new UnpStorageModel { Vunp = "0987654321", Vnaimp = "ОАО Белтелеком", Vnaimk = "Белтелеком", Vpadres = "г.Минск,пр-т.Независимости,1", Dreg = "1996-09-20", Nmns = "103", Vmns = "Инспекция МНС по Первомайскому району г.Минска", Ckodsost = "1", Dlikv = "null", Vlikv = "null", LastChecked = DateTime.Now },
                    new UnpStorageModel { Vunp = "987654321", Vnaimp = "ОАО МТС", Vnaimk = "МТС", Vpadres = "г.Минск,пр-т.Дзержинского,25", Dreg = "1997-05-12", Nmns = "105", Vmns = "Инспекция МНС по Ленинскому району г.Минска", Ckodsost = "1", Dlikv = "null", Vlikv = "null", LastChecked = DateTime.Now },
                    new UnpStorageModel { Vunp = "12345678", Vnaimp = "ОАО Беларуськалий", Vnaimk = "Беларуськалий", Vpadres = "г.Солигорск,ул.Калиевская,10", Dreg = "1998-03-18", Nmns = "106", Vmns = "Инспекция МНС по Фрунзенскому району г.Минска", Ckodsost = "1", Dlikv = "null", Vlikv = "null", LastChecked = DateTime.Now },
                    new UnpStorageModel { Vunp = "87654321", Vnaimp = "ОАО Минскводоканал", Vnaimk = "Минскводоканал", Vpadres = "г.Минск,ул.Немига,14", Dreg = "1999-11-25", Nmns = "107", Vmns = "Инспекция МНС по Советскому району г.Минска", Ckodsost = "1", Dlikv = "null", Vlikv = "null", LastChecked = DateTime.Now }
                );
                await SaveChangesAsync();
            }
        }
    }
}