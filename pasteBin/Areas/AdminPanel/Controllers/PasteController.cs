using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pasteBin.Areas.Home.Models;
using pasteBin.Database;
using pasteBin.Services.Interfaces;

namespace pasteBin.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = "Admin")]
    public class PasteController : Controller
    {
        private readonly DBContext db;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IRedisCache redis;

        public PasteController(DBContext contex, UserManager<IdentityUser> uManager, 
            RoleManager<IdentityRole> rManager, IRedisCache cache)
        {
            db = contex;
            userManager = uManager;
            roleManager = rManager;
            redis = cache;
        }

        [Route("Admin/Paste")]
        [HttpGet]
        public IActionResult Paste()
        {
            ViewBag.Host = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            return View(db.pasts.Include(a => a.Author));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePaste(string hash)
        {
            PasteModel paste = db.pasts.First(p => p.Hash == hash);

            IEnumerable<CommentModel> comments = db.comments.Where(c => c.Paste.Hash == paste.Hash).ToList();
            IEnumerable<LikesModel> likes = db.likes.Where(l => l.Paste.Hash == paste.Hash).ToList();
            IEnumerable<ReportModel> reports = db.reports.Where(r => r.Paste.Hash == paste.Hash).ToList();
            IEnumerable<ViewCheatModel> cheats = db.viewCheats.Where(c => c.Paste.Hash == paste.Hash).ToList();

            db.UpdateTables(comments, likes, reports, new List<PasteModel> { paste }, cheats);
            // в случае если пост остался так же в кеше чтобы не дожидаться его обновления
            redis.Remove(new List<PasteModel> { paste });

            return RedirectToAction("Paste");
        }
    }
}
