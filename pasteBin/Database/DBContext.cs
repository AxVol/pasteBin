using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using pasteBin.Areas.Home.Models;

namespace pasteBin.Database
{
    public class DBContext : IdentityDbContext
    {
        public DbSet<PasteModel> pasts { get; set; } = null!;
        public DbSet<CommentModel> comments { get; set; } = null!;
        public DbSet<LikesModel> likes { get; set; } = null!;
        public DbSet<ReportModel> reports { get; set; } = null!;
        public DbSet<ViewCheatModel> viewCheats { get; set; } = null!;

        // Обновляет в базе данных, данные из кеша для их актуальности
        public void UpdatePasteViews(IEnumerable<PasteModel> past)
        {
            foreach (PasteModel paste in past)
            {
                PasteModel trackedPaste = this.pasts.Find(paste.Id);

                if (!(paste.View == trackedPaste.View))
                {
                    trackedPaste.View = paste.View;

                    this.SaveChanges();
                }
            }
        }

        // чистить таблицы и её зависимости от переданных в списках данных
        public void UpdateTables(IEnumerable<CommentModel> comments, IEnumerable<LikesModel> likes,
            IEnumerable<ReportModel> reports, IEnumerable<PasteModel> pasts, IEnumerable<ViewCheatModel> viewCheat)
        {
            foreach (CommentModel comment in comments)
            {
                this.Entry(comment).State = EntityState.Deleted;
                this.SaveChanges();
            }

            foreach (LikesModel like in likes)
            {
                this.Entry(like).State = EntityState.Deleted;
                this.SaveChanges();
            }

            foreach (ReportModel report in reports)
            {
                this.Entry(report).State = EntityState.Deleted;
                this.SaveChanges();
            }

            foreach (ViewCheatModel cheat in viewCheat)
            {
                this.Entry(cheat).State = EntityState.Deleted;
                this.SaveChanges();
            }

            foreach (PasteModel paste in pasts)
            {
                this.Entry(paste).State = EntityState.Deleted;
                this.SaveChanges();
            }
        }

        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
