using Microsoft.AspNetCore.Mvc;

namespace SCMM.Controllers
{
    public class ActivitiesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
