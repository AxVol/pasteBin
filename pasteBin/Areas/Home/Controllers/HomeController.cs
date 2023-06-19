using Microsoft.AspNetCore.Mvc;
using pasteBin.Areas.Home.Models;

namespace pasteBin.Areas.Home.Controllers
{
    [Area("Home")]
    public class HomeController : Controller
    {
        [Route("")]
        [Route("Home")]
        public IActionResult Index()
        {
            PeopleModel people = new PeopleModel("Axvol", 21);
            return View(people);
        }
    }
}