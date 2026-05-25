using Microsoft.AspNetCore.Mvc;

namespace NhaXinh.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
