using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using pasteBin.Areas.Home.Models;
using pasteBin.Database;

namespace pasteBin.Services
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedHostedService> logger;
        private readonly DBContext dataBase;
        private readonly IRedis redis;
        private readonly int refreshInterval = 1;
        private readonly int cacheRefreshInterval = 1;
        private Timer? timer = null;

        public TimedHostedService(ILogger<TimedHostedService> log, IServiceScopeFactory factory)
        {
            logger = log;
            dataBase = factory.CreateScope().ServiceProvider.GetRequiredService<DBContext>();
            redis = factory.CreateScope().ServiceProvider.GetRequiredService<IRedis>();
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(refreshInterval));

            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            logger.LogInformation("-------------Очищаю базу данных от ненужных постов и связей-------------");
            DeleteTimePasts();
            logger.LogInformation("---------Обновляю кеш-----------");
            UpdateCache();
            logger.LogInformation("-----------Обноваляю сервис против накрутки-------------");
            UpdateCheatService();
        }

        private void DeleteTimePasts()
        {
            IEnumerable<PasteModel> pasts = dataBase.pasts.Where(p => p.DeleteDate < DateTime.Now).ToList();

            foreach (PasteModel paste in pasts)
            {
                IEnumerable<CommentModel> comments = dataBase.comments.Where(c => c.Paste == paste);
                IEnumerable<LikesModel> likes = dataBase.likes.Where(l => l.Paste == paste);
                IEnumerable<ReportModel> reports = dataBase.reports.Where(r => r.Paste == paste);
                IEnumerable<ViewCheatModel> viewCheats = dataBase.viewCheats.Where(v => v.Paste == paste);

                dataBase.UpdateTables(comments, likes, reports, new List<PasteModel> { paste }, viewCheats);

                redis.Remove(new List<PasteModel> { paste });

                logger.LogInformation($"Удалил - {paste.Title}");
            }
        }

        private void UpdateCache()
        {
            dataBase.UpdatePasteViews(redis.GetAll());

            IEnumerable<PasteModel> pasts = dataBase.pasts.Include(p => p.Author).OrderByDescending(p => p.View).Take(10);

            redis.Set(cacheRefreshInterval, pasts);
        }

        private void UpdateCheatService()
        {
            dataBase.viewCheats.ExecuteDelete();
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
