using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pasteBin.Areas.AdminPanel.ViewModels;
using pasteBin.Areas.Home.Models;
using pasteBin.Database;

namespace pasteBin.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class UserController : Controller
    {
        private readonly DBContext db;
        private readonly UserManager<IdentityUser> userManager;

        public UserController(DBContext contex, UserManager<IdentityUser> uManager)
        {
            db = contex;
            userManager = uManager;
        }

        [Route("Admin/Users")]
        [HttpGet]
        public IActionResult Users()
        {
            UsersViewModel viewModel = new(userManager.Users.ToList(), db);

            return View(viewModel);
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
                IEnumerable<ViewCheatModel> cheats = db.viewCheats.Where(v => v.User == user).ToList();

                await db.UpdateTables(comments, likes, reports, pasts, cheats);

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
    }
}
