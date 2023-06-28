using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using pasteBin.Areas.Home.Models;
using pasteBin.Services;
using pasteBin.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;
using System.Security.Policy;
using Azure.Core;
using Microsoft.AspNetCore.Http.Extensions;
using pasteBin.Areas.Home.ViewModels;

namespace pasteBin.Areas.Home.Controllers
{
    [Area("Home")]
    public class HomeController : Controller
    {
        private readonly IHashGenerator hashGenerator;
        private readonly DBContext dataBase;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public HomeController(IHashGenerator HashGenerator, DBContext context, 
            SignInManager<IdentityUser> sign, UserManager<IdentityUser> user)
        {
            this.hashGenerator = HashGenerator;
            this.dataBase = context;
            this.signInManager = sign;
            this.userManager = user;
        }

        [Route("")]
        [Route("Home")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (signInManager.IsSignedIn(User))
            {
                ViewBag.IsSignedIn = true;

                return View();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(PasteModel paste)
        {
            if (!ModelState.IsValid)
            {
                if (paste.Text == null)
                {
                    ViewBag.Text = "Введите хоть что-то... = (";
                }

                if (paste.Title == null)
                {
                    ViewBag.Name = "Введите название";
                }

                return View();
            }

            if (!signInManager.IsSignedIn(User))
            {
                return View();
            }

            ViewBag.IsSignedIn = true;

            string hash = hashGenerator.HashForURL();
            string? action = Url.Action("Paste", new { hash = $"{hash}" });
            string url = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{action}";

            ViewBag.UrlToPaste = url;

            paste.Hash = hash;
            paste.Author = await userManager.GetUserAsync(HttpContext.User);

            dataBase.pasts.Add(paste);

            await dataBase.SaveChangesAsync();

            return View();
        }

        [Route("Paste/{hash}")]
        public async Task<IActionResult> Paste(string hash)
        {
            PasteModel? paste = dataBase.pasts.Include(a => a.Author).FirstOrDefault(p => p.Hash == hash);

            if (paste == null)
                return NotFound();

            if (signInManager.IsSignedIn(User))
            {
                paste.View++;
            }
            
            IEnumerable<CommentModel> comments = dataBase.comments.Include(a => a.Author).Where(c => c.Paste == paste);
            IEnumerable<LikesModel> likes = dataBase.likes.Include(p => p.Paste).Where(p => p.Paste == paste);
            
            PasteViewModel viewModel = new PasteViewModel(paste, comments, likes);

            await dataBase.SaveChangesAsync();

            return View(viewModel);
        }

        [Route("Paste/Popular")]
        public async Task<IActionResult> Popular()
        {
            IEnumerable<PasteModel> pasts = dataBase.pasts.Include(a => a.Author).OrderByDescending(a => a.View).Take(10);

            ViewBag.Host = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            return View(pasts);
        }

        public async Task<IActionResult> AddComment(string com, string hash)
        {
            string pasteHash = hash;
            IdentityUser user = await userManager.GetUserAsync(HttpContext.User);
            PasteModel paste = await dataBase.pasts.FirstAsync(p => p.Hash == pasteHash);

            CommentModel comment = new CommentModel();
            comment.Author = user;
            comment.Paste = paste;
            comment.Comment = com;

            dataBase.comments.Add(comment);
            await dataBase.SaveChangesAsync();

            return RedirectToAction("Paste", new {hash = hash});
        }

        public async Task<IActionResult> AddLike(string hash)
        {
            string pasteHash = hash;
            IdentityUser user = await userManager.GetUserAsync(HttpContext.User);
            PasteModel paste = await dataBase.pasts.FirstAsync(p => p.Hash == pasteHash);

            LikesModel like = new LikesModel();
            like.Paste = paste;
            like.User = user;

            dataBase.likes.Add(like);
            await dataBase.SaveChangesAsync();

            return RedirectToAction("Paste", new { hash = hash });
        }
        
        public async Task<IActionResult> RemoveLike(string hash)
        {
            string pasteHash = hash;
            IdentityUser user = await userManager.GetUserAsync(HttpContext.User);
            PasteModel paste = await dataBase.pasts.FirstAsync(p => p.Hash == pasteHash);
            LikesModel like = await dataBase.likes.FirstAsync(l => l.User == user && l.Paste == paste);

            dataBase.Entry(like).State = EntityState.Deleted;
            dataBase.SaveChanges();

            return RedirectToAction("Paste", new { hash = hash });
        }
    }
}