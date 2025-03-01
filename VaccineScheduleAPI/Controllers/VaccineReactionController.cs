using Microsoft.AspNetCore.Mvc;

namespace VaccineScheduleAPI.Controllers
{
    public class VaccineReactionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
