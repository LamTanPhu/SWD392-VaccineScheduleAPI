using Microsoft.AspNetCore.Mvc;

namespace VaccineScheduleAPI.Controllers
{
    public class ChildrenProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
