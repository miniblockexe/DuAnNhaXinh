using Microsoft.AspNetCore.Mvc;

namespace NhaXinh.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
