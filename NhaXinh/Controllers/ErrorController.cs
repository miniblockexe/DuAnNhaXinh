using Microsoft.AspNetCore.Mvc;

namespace NhaXinh.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
