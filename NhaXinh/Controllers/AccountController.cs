using Microsoft.AspNetCore.Mvc;

namespace NhaXinh.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
