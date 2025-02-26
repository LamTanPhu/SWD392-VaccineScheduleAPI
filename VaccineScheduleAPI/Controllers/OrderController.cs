using Microsoft.AspNetCore.Mvc;

namespace VaccineScheduleAPI.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
