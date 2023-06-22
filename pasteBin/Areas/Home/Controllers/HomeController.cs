using Microsoft.AspNetCore.Mvc;
using pasteBin.Areas.Home.Models;
using pasteBin.Services;
using pasteBin.Database;

namespace pasteBin.Areas.Home.Controllers
{
    [Area("Home")]
    public class HomeController : Controller
    {
        private IHashGenerator hashGenerator;
        private DBContext dataBase;

        public HomeController(IHashGenerator HashGenerator, DBContext context)
        {
            this.hashGenerator = HashGenerator;
            this.dataBase = context;
        }

        [Route("")]
        [Route("Home")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(PasteModel paste)
        {
            if (ModelState.IsValid)
            {
                string hash = hashGenerator.HashForURL();
                string? action = Url.Action("Paste", new { hash = $"{hash}" });
                string url = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{action}";

                ViewBag.UrlToPaste = url;

                dataBase.pasts.Add(paste);
                await dataBase.SaveChangesAsync();

                return View();
            }

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

        [Route("Paste/{hash}")]
        public IActionResult Paste(string hash)
        {
            ViewBag.Hash = hash;

            return View();
        }
    }
}