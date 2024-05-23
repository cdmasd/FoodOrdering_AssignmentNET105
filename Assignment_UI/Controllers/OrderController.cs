using Microsoft.AspNetCore.Mvc;

namespace Assignment_UI.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
