using Microsoft.AspNetCore.Mvc;

namespace VaccineScheduleAPI.Controllers
{
    public class VaccineHistoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
