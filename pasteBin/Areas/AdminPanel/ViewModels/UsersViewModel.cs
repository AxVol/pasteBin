using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using pasteBin.Areas.Home.Models;
using pasteBin.Database;

namespace pasteBin.Areas.AdminPanel.ViewModels
{
    public class UsersViewModel
    {
        public IEnumerable<IdentityUser> Users { get; set; }
        public DBContext dataBase { get; set; }

        public UsersViewModel(IEnumerable<IdentityUser> users, DBContext db)
        {
            Users = users;
            dataBase = db;
        }

        public int GetLikesCount(IdentityUser user)
        {
            int count = 0;
            IEnumerable<PasteModel> pasts = dataBase.pasts.Include(p => p.Author).Where(p => p.Author == user).ToList();
            
            foreach (PasteModel paste in pasts)
            {
                count += dataBase.likes.Where(l => l.Paste == paste).Count();
            }

            return count;
        }

        public bool HaveReports(IdentityUser user)
        {
            IEnumerable<ReportModel> reports = dataBase.reports.Where(r => r.User == user);

            if (reports.Any())
                return true;

            return false;
        }
    }
}
