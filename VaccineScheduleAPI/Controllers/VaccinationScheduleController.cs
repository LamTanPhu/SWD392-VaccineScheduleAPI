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

        // CREATE Order Schedule
        [Authorize(Roles = "Admin, Staff")]
        [HttpPost("order")]
        public async Task<ActionResult<List<ScheduleResponseDTO>>> CreateOrderSchedules([FromBody] ScheduleRequestDTO request)
        {
            var createdSchedules = await _scheduleService.CreateSchedulesAsync(request);
            return Ok(createdSchedules);
        }

        // CREATE Schedule
        [Authorize(Roles = "Admin, Staff")]
        [HttpPost]
        public async Task<ActionResult<ScheduleResponseDTO>> CreateSchedule([FromBody] CreateScheduleRequestDTO request)
        {
            var createdSchedule = await _scheduleService.CreateScheduleAsync(request);
            return CreatedAtAction(nameof(GetScheduleById), new { id = createdSchedule.Id }, createdSchedule);
        }

        // READ ALL
        [Authorize(Roles = "Admin, Staff")]
        [HttpGet]
        public async Task<ActionResult<List<ScheduleResponseDTO>>> GetAllSchedules()
        {
            var schedules = await _scheduleService.GetAllSchedulesAsync();
            return Ok(schedules);
        }

        // READ BY ID
        [Authorize(Roles = "Admin, Staff")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ScheduleResponseDTO>> GetScheduleById(string id)
        {
            var schedule = await _scheduleService.GetScheduleByIdAsync(id);
            return Ok(schedule);
        }

        // UPDATE
        [Authorize(Roles = "Admin, Staff")]
        [HttpPut]
        public async Task<IActionResult> UpdateSchedule(string scheduleId, [FromBody] UpdateScheduleRequestDTO request)
        {
            await _scheduleService.UpdateScheduleAsync(scheduleId, request);
            return NoContent();
        }

        // DELETE (Soft Delete)
        [Authorize(Roles = "Admin, Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(string id)
        {
            await _scheduleService.DeleteScheduleAsync(id);
            return NoContent();
        }
    }
}
