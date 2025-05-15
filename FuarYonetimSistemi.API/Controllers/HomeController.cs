using Microsoft.AspNetCore.Mvc;

namespace FuarYonetimSistemi.API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
