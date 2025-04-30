using Microsoft.AspNetCore.Mvc;

namespace SCMM.Controllers
{
    public class CoursesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
