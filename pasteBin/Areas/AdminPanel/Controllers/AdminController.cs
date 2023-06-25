using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Users() => View(userManager.Users.ToList());

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
            
            db.Entry(paste).State = EntityState.Deleted;
            await db.SaveChangesAsync();

            return RedirectToAction("Paste");
        }
    }
}
