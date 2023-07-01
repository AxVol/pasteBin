using Microsoft.EntityFrameworkCore;
using pasteBin.Areas.Home.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace pasteBin.Database
{
    public class DBContext : IdentityDbContext
    {
        public DbSet<PasteModel> pasts { get; set; } = null!;
        public DbSet<CommentModel> comments { get; set; } = null!;
        public DbSet<LikesModel> likes { get; set; } = null!;
        public DbSet<ReportModel> reports { get; set; } = null!;
        public DbSet<ViewCheatModel> viewCheats { get; set; } = null!;

        public DBContext(DbContextOptions<DBContext> options) : base(options) 
        {
            
        }

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
    }
}
