using Quartz;
using UNP.Data;
using UNP.Models;
using UNP.Services;

namespace UNP.Tasks
{
    public class CheckUnpJob : IJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpClientFactory _httpClientFactory;

        public CheckUnpJob(IServiceScopeFactory scopeFactory, IHttpClientFactory httpClientFactory)
        {
            _scopeFactory = scopeFactory;
            _httpClientFactory = httpClientFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var unpHistories = dbContext.UnpHistories.ToList();

            foreach (var history in unpHistories)
            {
                bool isInLocalDb = dbContext.UnpDatas.Any(u => u.Vunp == history.Unp);

                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync($"http://grp.nalog.gov.by/api/grp-public/data?unp={history.Unp}&charset=UTF-8&type=json");
                bool isInExternalDb = response.IsSuccessStatusCode;

                if (history.IsInLocalDb != isInLocalDb || history.IsInExternalDb != isInExternalDb)
                {
                    history.IsInLocalDb = isInLocalDb;
                    history.IsInExternalDb = isInExternalDb;
                    history.LastChecked = DateTime.Now;

                    // Запоминаем изменения для уведомления пользователя
                    await dbContext.UnpHistoryChanges.AddAsync(new UnpHistoryChange
                    {
                        Unp = history.Unp,
                        Email = history.Email,
                        ChangeDate = DateTime.Now,
                        ChangeType = "Status changed"
                    });
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }

}
