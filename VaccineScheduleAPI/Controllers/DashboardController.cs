using IServices.Interfaces.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Responses.Dashboard;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] 
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("overview")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DashboardResponseDTO>> GetDashboard()
        {
            var dashboardData = await _dashboardService.GetDashboardDataAsync();
            return Ok(dashboardData);
        }

        [HttpGet("revenue-orders/day")]
        public async Task<ActionResult<IEnumerable<RevenueAndOrderResponseDTO>>> GetRevenueAndOrdersByDay(
            [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
                return BadRequest("Start date must be less than or equal to end date.");

            var data = await _dashboardService.GetRevenueAndOrdersByDayAsync(startDate, endDate);
            return Ok(data);
        }

        [HttpGet("revenue-orders/month")]
        public async Task<ActionResult<IEnumerable<RevenueAndOrderResponseDTO>>> GetRevenueAndOrdersByMonth(
            [FromQuery] int year)
        {
            if (year < 2000 || year > DateTime.Now.Year)
                return BadRequest("Invalid year.");

            var data = await _dashboardService.GetRevenueAndOrdersByMonthAsync(year);
            return Ok(data);
        }

        [HttpGet("revenue-orders/year")]
        public async Task<ActionResult<IEnumerable<RevenueAndOrderResponseDTO>>> GetRevenueAndOrdersByYear(
            [FromQuery] int startYear, [FromQuery] int endYear)
        {
            if (startYear > endYear || startYear < 2000 || endYear > DateTime.Now.Year)
                return BadRequest("Invalid year range.");

            var data = await _dashboardService.GetRevenueAndOrdersByYearAsync(startYear, endYear);
            return Ok(data);
        }

    }
}
