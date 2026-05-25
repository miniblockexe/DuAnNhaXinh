using Microsoft.AspNetCore.Mvc;

namespace NhaXinh.Areas.Admin.Controllers
{
    public class BannerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
