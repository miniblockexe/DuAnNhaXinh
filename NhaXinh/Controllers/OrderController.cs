using Microsoft.AspNetCore.Mvc;

namespace NhaXinh.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
