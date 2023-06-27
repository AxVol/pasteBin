using Microsoft.EntityFrameworkCore;
using pasteBin.Areas.Home.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace pasteBin.Database
{
    public class DBContext : IdentityDbContext
    {
        public DbSet<PasteModel> pasts { get; set; } = null!;
        public DbSet<CommentModel> comments { get; set; } = null!;

        public DBContext(DbContextOptions<DBContext> options) : base(options) 
        {
            
        }
    }
}
