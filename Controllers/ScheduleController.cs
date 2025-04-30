using Microsoft.AspNetCore.Mvc;

namespace SCMM.Controllers
{
    public class ScheduleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
