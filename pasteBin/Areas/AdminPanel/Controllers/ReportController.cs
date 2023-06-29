using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pasteBin.Areas.Home.Models;
using pasteBin.Database;

namespace pasteBin.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class ReportController : Controller
    {
        private readonly DBContext db;

        public ReportController(DBContext contex)
        {
            db = contex;
        }

        public IActionResult Reports(string id)
        {
            IEnumerable<ReportModel> reports = db.reports.Include(u => u.User).Where(r => r.User.Id == id);

            return View(reports);
        }

        public async Task<IActionResult> RemoveReport(int id)
        {
            ReportModel report = await db.reports.Include(r => r.User).FirstAsync(r => r.Id == id);
            string user = report.User.Id;

            db.Entry(report).State = EntityState.Deleted;
            await db.SaveChangesAsync();

            return RedirectToAction("Reports", new { id = user });
        }
    }
}
