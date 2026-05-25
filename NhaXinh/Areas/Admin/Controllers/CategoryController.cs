using Microsoft.AspNetCore.Mvc;

namespace NhaXinh.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
