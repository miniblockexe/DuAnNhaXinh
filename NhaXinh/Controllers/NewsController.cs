using Microsoft.AspNetCore.Mvc;

namespace NhaXinh.Controllers
{
    public class NewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
