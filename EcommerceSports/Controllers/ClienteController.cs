using Microsoft.AspNetCore.Mvc;

namespace EcommerceSports.Controllers
{
    public class ClienteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
