using Microsoft.AspNetCore.Mvc;

namespace SCMM.Controllers
{
    public class ComplaintsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
