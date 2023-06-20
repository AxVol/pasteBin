using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using pasteBin.Areas.Home.Models;

namespace pasteBin.Areas.Home.Controllers
{
    [Area("Home")]
    public class HomeController : Controller
    {
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

        }

        [Route("Paste")]
        [HttpPost]
        public IActionResult Article(PasteModel paste)
        {
            if (ModelState.IsValid)
                return View(paste);

            string errorMessage = String.Empty;
            
            foreach (var item in ModelState)
            {
                if (item.Value.ValidationState == ModelValidationState.Invalid)
                {
                    errorMessage += $"{errorMessage} \n Ошибки {item.Key}";

                    foreach (var error in item.Value.Errors)
                    {
                        errorMessage += $"{errorMessage} - {error.ErrorMessage}\n";
                    }
                }
            }

            return View(errorMessage);
        }
    }
}