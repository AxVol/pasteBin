using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using pasteBin.Database;
using pasteBin.Areas.Home.Models;
using pasteBin.Services.Interfaces;

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
            string? action = Url.RouteUrl( new { controller = "Paste", action = "paste",  hash = $"{hash}" });
            string url = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{action}";

            ViewBag.UrlToPaste = url;

            paste.Hash = hash;
            paste.Author = await userManager.GetUserAsync(HttpContext.User);

            dataBase.pasts.Add(paste);
            await dataBase.SaveChangesAsync();

            return View();
        }
    }
}