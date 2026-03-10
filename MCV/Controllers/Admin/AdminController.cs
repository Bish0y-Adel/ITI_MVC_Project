using Microsoft.AspNetCore.Mvc;

namespace MCV.Controllers.Admin
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
