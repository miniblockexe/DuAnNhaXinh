using Microsoft.AspNetCore.Mvc;

namespace NhaXinh.Areas.Admin.Controllers
{
    public class NewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
