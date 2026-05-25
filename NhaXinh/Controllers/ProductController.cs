using Microsoft.AspNetCore.Mvc;

namespace NhaXinh.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
