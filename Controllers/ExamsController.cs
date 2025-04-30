using Microsoft.AspNetCore.Mvc;

namespace SCMM.Controllers
{
    public class ExamsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
