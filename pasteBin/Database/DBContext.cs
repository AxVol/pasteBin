using Microsoft.EntityFrameworkCore;
using pasteBin.Areas.Home.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Composition;

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

        public async Task UpdateTables(IEnumerable<CommentModel> comments, IEnumerable<LikesModel> likes,
            IEnumerable<ReportModel> reports, IEnumerable<PasteModel> pasts, IEnumerable<ViewCheatModel> viewCheat)
        {
            foreach (CommentModel comment in comments)
            {
                this.Entry(comment).State = EntityState.Deleted;
                await this.SaveChangesAsync();
            }

            foreach (LikesModel like in likes)
            {
                this.Entry(like).State = EntityState.Deleted;
                await this.SaveChangesAsync();
            }

            foreach (ReportModel report in reports)
            {
                this.Entry(report).State = EntityState.Deleted;
                await this.SaveChangesAsync();
            }

            foreach (ViewCheatModel cheat in viewCheat)
            {
                this.Entry(cheat).State = EntityState.Deleted;
                await this.SaveChangesAsync();
            }

            foreach (PasteModel paste in pasts)
            {
                this.Entry(paste).State = EntityState.Deleted;
                await this.SaveChangesAsync();
            }

            await Task.CompletedTask;
        }
    }
}
