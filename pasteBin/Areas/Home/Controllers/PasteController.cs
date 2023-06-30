using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pasteBin.Database;
using pasteBin.Areas.Home.Models;
using pasteBin.Areas.Home.ViewModels;

namespace pasteBin.Areas.Home.Controllers
{
    [Area("Home")]
    public class PasteController : Controller
    {
        private readonly DBContext dataBase;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public PasteController(DBContext context, SignInManager<IdentityUser> sign, 
            UserManager<IdentityUser> user)
        {
            this.dataBase = context;
            this.signInManager = sign;
            this.userManager = user;
        }

        [Route("Paste/{hash}")]
        public async Task<IActionResult> Paste(string hash)
        {
            PasteModel paste = await dataBase.pasts.Include(a => a.Author).FirstAsync(p => p.Hash == hash);

            if (paste == null)
                return NotFound();

            IEnumerable<CommentModel> comments = dataBase.comments.Include(a => a.Author).Where(c => c.Paste == paste);
            IEnumerable<LikesModel> likes = dataBase.likes.Include(p => p.Paste).Include(u => u.User).Where(p => p.Paste == paste);
            
            await dataBase.SaveChangesAsync();

            PasteViewModel viewModel = new (paste, comments, likes);

            if (signInManager.IsSignedIn(User))
            {
                IdentityUser user = await userManager.GetUserAsync(HttpContext.User);

                IEnumerable<ViewCheatModel> cheat = dataBase.viewCheats.Include(u => u.User).
                    Where(c => c.Paste == paste && c.User == user);

                if (!cheat.Any())
                    paste.View++;

                ViewCheatModel viewCheat = new ViewCheatModel();
                viewCheat.Paste = paste;
                viewCheat.User = user;

                dataBase.viewCheats.Add(viewCheat);
                await dataBase.SaveChangesAsync();
            }

            return View(viewModel);
        }

        [Route("Paste/Popular")]
        public IActionResult Popular()
        {
            IEnumerable<PasteModel> pasts = dataBase.pasts.Include(a => a.Author).OrderByDescending(a => a.View).Take(10);

            ViewBag.Host = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            return View(pasts);
        }

        public async Task<IActionResult> AddComment(string com, string hash)
        {
            if (com == null)
            {
                return RedirectToAction("Paste", new { hash = hash });
            }

            IdentityUser user = await userManager.GetUserAsync(HttpContext.User);
            PasteModel paste = await dataBase.pasts.FirstAsync(p => p.Hash == hash);

            CommentModel comment = new();
            comment.Author = user;
            comment.Paste = paste;
            comment.Comment = com;

            dataBase.comments.Add(comment);
            await dataBase.SaveChangesAsync();

            return RedirectToAction("Paste", new { hash = hash });
        }

        public async Task<IActionResult> AddLike(string hash)
        {
            IdentityUser user = await userManager.GetUserAsync(HttpContext.User);
            PasteModel paste = await dataBase.pasts.FirstAsync(p => p.Hash == hash);

            LikesModel like = new();
            like.Paste = paste;
            like.User = user;

            dataBase.likes.Add(like);
            await dataBase.SaveChangesAsync();

            return RedirectToAction("Paste", new { hash = hash });
        }

        public async Task<IActionResult> RemoveLike(string hash)
        {
            IdentityUser user = await userManager.GetUserAsync(HttpContext.User);
            PasteModel paste = await dataBase.pasts.FirstAsync(p => p.Hash == hash);
            LikesModel like = await dataBase.likes.FirstAsync(l => l.User == user && l.Paste == paste);

            dataBase.Entry(like).State = EntityState.Deleted;
            dataBase.SaveChanges();

            return RedirectToAction("Paste", new { hash = hash });
        }

        public async Task<IActionResult> SendReport(string reportText, string hash)
        {
            if (reportText == null)
                return RedirectToAction("Paste", new { hash = hash });

            PasteModel paste = await dataBase.pasts.Include(a => a.Author).FirstAsync(p => p.Hash == hash);

            ReportModel report = new();
            report.ReportText = reportText;
            report.User = paste.Author;
            report.Paste = paste;

            dataBase.reports.Add(report);
            await dataBase.SaveChangesAsync();

            return RedirectToAction("Paste", new { hash = hash });
        }
    }
}
