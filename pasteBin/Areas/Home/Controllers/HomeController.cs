using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using pasteBin.Areas.Home.Models;
using pasteBin.Services;
using pasteBin.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace pasteBin.Areas.Home.Controllers
{
    [Area("Home")]
    public class HomeController : Controller
    {
        private readonly IHashGenerator hashGenerator;
        private readonly DBContext dataBase;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public HomeController(IHashGenerator HashGenerator, DBContext context, SignInManager<IdentityUser> sign, UserManager<IdentityUser> user)
        {
            this.hashGenerator = HashGenerator;
            this.dataBase = context;
            this.signInManager = sign;
            this.userManager = user;
        }

        [Route("")]
        [Route("Home")]
        [HttpGet]
        public IActionResult Index()
        {
            if (signInManager.IsSignedIn(User))
            {
                ViewBag.IsSignedIn = true;

                return View();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(PasteModel paste, IdentityUser user)
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

            string hash = hashGenerator.HashForURL();
            string? action = Url.Action("Paste", new { hash = $"{hash}" });
            string url = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{action}";

            ViewBag.UrlToPaste = url;

            paste.Hash = hash;
            paste.Author = user;

            dataBase.pasts.Add(paste);

            await dataBase.SaveChangesAsync();

            return View();
        }

        [Route("Paste/{hash}")]
        public IActionResult Paste(string hash)
        {
            PasteModel? paste = dataBase.pasts.Include(a => a.Author).FirstOrDefault(p => p.Hash == hash);

            if (paste == null)
                return NotFound();

            paste.View++;
            dataBase.SaveChanges();

            return View(paste);
        }
    }
}