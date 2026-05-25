using Microsoft.AspNetCore.Mvc;

namespace NhaXinh.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
