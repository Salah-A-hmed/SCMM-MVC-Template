using Microsoft.AspNetCore.Mvc;

namespace SCMM.Controllers
{
    public class EventsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
