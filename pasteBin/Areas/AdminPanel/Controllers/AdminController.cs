using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pasteBin.Areas.AdminPanel.ViewModels;
using pasteBin.Areas.Home.Models;
using pasteBin.Database;

namespace pasteBin.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly DBContext db;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AdminController(DBContext contex, UserManager<IdentityUser> uManager, RoleManager<IdentityRole> rManager)
        {
            db = contex;
            userManager = uManager;
            roleManager = rManager;
        }

        [Route("Admin/Users")]
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            UsersViewModel viewModel = new UsersViewModel(userManager.Users.ToList(), db);

            return View(viewModel);
        }

            [Route("Admin/Paste")]
        [HttpGet]
        public async Task<IActionResult> Paste()
        {
            ViewBag.Host = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            return View(db.pasts.Include(a => a.Author));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            IdentityUser? user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                IEnumerable<PasteModel> pasts = db.pasts.Where(p => p.Author == user).ToList();
                IEnumerable<CommentModel> comments = db.comments.Where(c => c.Author == user).ToList();
                IEnumerable<LikesModel> likes = db.likes.Where(l => l.User == user).ToList();
                IEnumerable<ReportModel> reports = db.reports.Where(r => r.User == user).ToList();

                foreach (CommentModel comment in comments)
                {
                    db.Entry(comment).State = EntityState.Deleted;
                    await db.SaveChangesAsync();
                }

                foreach (LikesModel like in likes)
                {
                    db.Entry(like).State = EntityState.Deleted;
                    await db.SaveChangesAsync();
                }

                foreach (ReportModel report in reports)
                {
                    db.Entry(report).State = EntityState.Deleted;
                    await db.SaveChangesAsync();
                }

                foreach (PasteModel paste in pasts)
                {
                    db.Entry(paste).State = EntityState.Deleted;
                    await db.SaveChangesAsync();
                }

                IdentityResult result = await userManager.DeleteAsync(user);
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> UpToAdmin(string id)
        {
            IdentityUser? user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                await userManager.RemoveFromRoleAsync(user, "User");
                await userManager.AddToRoleAsync(user, "Admin");
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePaste(int id)
        {
            PasteModel paste = new PasteModel { Id = id };

            IEnumerable<CommentModel> comments = db.comments.Where(c => c.Paste.Id == paste.Id).ToList();
            IEnumerable<LikesModel> likes = db.likes.Where(l => l.Paste.Id == paste.Id).ToList();
            IEnumerable<ReportModel> reports = db.reports.Where(r => r.Paste.Id == paste.Id).ToList();

            foreach (CommentModel comment in comments)
            {
                db.Entry(comment).State = EntityState.Deleted;
                await db.SaveChangesAsync();
            }

            foreach (LikesModel like in likes)
            {
                db.Entry(like).State = EntityState.Deleted;
                await db.SaveChangesAsync();
            }

            foreach (ReportModel report in reports)
            {
                db.Entry(report).State = EntityState.Deleted;
                await db.SaveChangesAsync();
            }

            db.Entry(paste).State = EntityState.Deleted;
            await db.SaveChangesAsync();

            return RedirectToAction("Paste");
        }
    }
}
