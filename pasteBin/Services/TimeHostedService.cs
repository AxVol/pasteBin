using Microsoft.EntityFrameworkCore;
using pasteBin.Areas.Home.Models;
using pasteBin.Database;

namespace pasteBin.Services
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedHostedService> logger;
        private readonly DBContext dataBase;
        private Timer? timer = null;

        public TimedHostedService(ILogger<TimedHostedService> log, IServiceScopeFactory factory)
        {
            logger = log;
            dataBase = factory.CreateScope().ServiceProvider.GetRequiredService<DBContext>();
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            logger.LogInformation("-------------Очищаю базу данных от ненужных постов и связей-------------");
            DeleteTimePasts();
            logger.LogInformation("---------Перезаписываю кеш-----------");
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

                foreach (CommentModel comment in comments)
                {
                    dataBase.Entry(comment).State = EntityState.Deleted;
                    dataBase.SaveChanges();
                }

                foreach (LikesModel like in likes)
                {
                    dataBase.Entry(like).State = EntityState.Deleted;
                    dataBase.SaveChanges();
                }

                foreach (ReportModel report in reports)
                {
                    dataBase.Entry(report).State = EntityState.Deleted;
                    dataBase.SaveChanges();
                }

                dataBase.Entry(paste).State = EntityState.Deleted;
                dataBase.SaveChanges();

                logger.LogInformation($"Удалил - {paste.Title}");
            }
        }

        private void UpdateCache()
        {
            logger.LogInformation($"{nameof(UpdateCache)}");
        }

        private void UpdateCheatService()
        {
            logger.LogInformation($"{nameof(UpdateCheatService)}");
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
