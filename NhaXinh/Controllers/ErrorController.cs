using Microsoft.AspNetCore.Mvc;

namespace NhaXinh.Controllers
{
    public class ErrorController : Controller
    {
        [Route("loi/{statusCode:int}")]
        public IActionResult Handle(int statusCode)
        {
            Response.StatusCode = statusCode;
            return statusCode switch
            {
                404 => View("404"),
                500 => View("500"),
                _ => View("500")
            };
        }

        [Route("loi/500")]
        public IActionResult ServerError()
        {
            Response.StatusCode = 500;
            return View("500");
        }
    }
}
