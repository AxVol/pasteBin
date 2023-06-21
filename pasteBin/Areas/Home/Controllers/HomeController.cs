using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using pasteBin.Areas.Home.Models;
using pasteBin.Services;

namespace pasteBin.Areas.Home.Controllers
{
    [Area("Home")]
    public class HomeController : Controller
    {
        private IHashGenerator hashGenerator;

        public HomeController(IHashGenerator HashGenerator)
        {
            this.hashGenerator = HashGenerator;
        }

        [Route("")]
        [Route("Home")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(PasteModel paste)
        {
            if (ModelState.IsValid)
            {
                string hash = hashGenerator.HashForURL();
                string? action = Url.Action("Paste", new { hash = $"{hash}" });
                string url = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{action}";

                ViewBag.UrlToPaste = url;

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