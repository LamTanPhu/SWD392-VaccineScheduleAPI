using IServices.Interfaces.Schedules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.Schedule;
using ModelViews.Responses.Schedule;
using Services.Services.Schedules;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaccinationScheduleController : ControllerBase
    {
        private readonly IVaccinationScheduleService _scheduleService;

        public VaccinationScheduleController(IVaccinationScheduleService scheduleService)
        {
            _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
        }


        [Authorize(Roles = "Admin, Staff, Parent")]
        [HttpPost("order")]
        public async Task<ActionResult<List<ScheduleResponseDTO>>> CreateOrderSchedules([FromBody] ScheduleRequestDTO request)
        {
            var createdSchedules = await _scheduleService.CreateSchedulesAsync(request);
            return Ok(createdSchedules);
        }


        [Authorize(Roles = "Admin, Staff, Parent")]
        [HttpPost]
        public async Task<ActionResult<ScheduleResponseDTO>> CreateSchedule([FromBody] CreateScheduleRequestDTO request)
        {
            var createdSchedule = await _scheduleService.CreateScheduleAsync(request);
            return CreatedAtAction(nameof(GetScheduleById), new { id = createdSchedule.Id }, createdSchedule);
        }


        [Authorize(Roles = "Admin, Staff, Parent")]
        [HttpGet]
        public async Task<ActionResult<List<ScheduleResponseDTO>>> GetAllSchedules()
        {
            var schedules = await _scheduleService.GetAllSchedulesAsync();
            return Ok(schedules);
        }


        [Authorize(Roles = "Admin, Staff, Parent")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ScheduleResponseDTO>> GetScheduleById(string id)
        {
            var schedule = await _scheduleService.GetScheduleByIdAsync(id);
            return Ok(schedule);
        }


        [Authorize(Roles = "Admin, Staff, Parent")]
        [HttpPut]
        public async Task<IActionResult> UpdateSchedule(string scheduleId, [FromBody] UpdateScheduleRequestDTO request)
        {
            await _scheduleService.UpdateScheduleAsync(scheduleId, request);
            return NoContent();
        }

    
        [Authorize(Roles = "Admin, Staff, Parent")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(string id)
        {
            await _scheduleService.DeleteScheduleAsync(id);
            return NoContent();
        }
    }
}
