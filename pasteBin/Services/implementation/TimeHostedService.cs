using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using pasteBin.Areas.Home.Models;
using pasteBin.Database;
using pasteBin.Services.Interfaces;

namespace pasteBin.Services.implementation
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedHostedService> logger;
        private readonly IRedisCache redis;
        private readonly int refreshInterval = 1;
        private readonly DBContext dataBase;
        private Timer? timer = null;

        public TimedHostedService(ILogger<TimedHostedService> log, IServiceScopeFactory factory)
        {
            logger = log;
            dataBase = factory.CreateScope().ServiceProvider.GetRequiredService<DBContext>();
            redis = factory.CreateScope().ServiceProvider.GetRequiredService<IRedisCache>();
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromDays(refreshInterval));

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

        // Удаляет посты у которых настала дата удаления
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

                logger.LogInformation($"Удалил - {paste.Title}");
            }

            redis.Remove(pasts);
        }

        private void UpdateCache()
        {
            dataBase.UpdatePasteViews(redis.GetAll());

            IEnumerable<PasteModel> pasts = dataBase.pasts.Include(p => p.Author).OrderByDescending(p => p.View).Take(10);

            redis.Set(refreshInterval, pasts);
        }

        /* по средствам базы данных и отдельной в ней таблицы связанной с юзером и постом, отслеживаю смотрел ли
           пользователь в течении дня пост, чтобы не начислять лишние просмотры */
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
