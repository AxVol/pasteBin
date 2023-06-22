using Microsoft.EntityFrameworkCore;
using pasteBin.Areas;
using pasteBin.Areas.Home.Controllers;
using pasteBin.Areas.Home.Models;

namespace pasteBin.Database
{
    public class DBContext : DbContext
    {
        public DbSet<PasteModel> pasts { get; set; } = null!;

        public DBContext(DbContextOptions<DBContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }
    }
}
