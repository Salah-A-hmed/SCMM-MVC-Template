using Microsoft.AspNetCore.Mvc;

namespace SCMM.Controllers
{
    public class LibraryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
