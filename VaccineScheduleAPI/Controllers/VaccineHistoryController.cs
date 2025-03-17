using IServices.Interfaces.Schedules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.VaccineHistory;
using ModelViews.Responses.VaccineHistory;
using System.Security.Claims;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaccineHistoryController : ControllerBase
    {
        private readonly IVaccineHistoryService _vaccineHistoryService;

        public VaccineHistoryController(IVaccineHistoryService vaccineHistoryService)
        {
            _vaccineHistoryService = vaccineHistoryService ?? throw new ArgumentNullException(nameof(vaccineHistoryService));
        }

        //[Authorize(Roles = "Parent, Admin")]
        [HttpPost("create")]
        public async Task<ActionResult<VaccineHistoryResponseDTO>> CreateVaccineHistory([FromBody] CreateVaccineHistoryRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var accountId = User.FindFirst("Id")?.Value; // Lấy AccountId từ JWT
            var response = await _vaccineHistoryService.CreateVaccineHistoryAsync(request, accountId);
            return CreatedAtAction(nameof(CreateVaccineHistory), new { id = response.Id }, response);
        }
        //--------------------------------------------------------------------------------------

        //[Authorize(Roles = "Parent, Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<VaccineHistoryResponseDTO>> GetVaccineHistoryById(string id)
        {
            var response = await _vaccineHistoryService.GetVaccineHistoryByIdAsync(id);
            if (response == null)
                return NotFound("Không tìm thấy lịch sử vaccine.");

            return Ok(response);
        }

        //[Authorize(Roles = "Parent, Admin")]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<VaccineHistoryResponseDTO>>> GetAllVaccineHistories()
        {
            var response = await _vaccineHistoryService.GetAllVaccineHistoriesAsync();
            return Ok(response);
        }

        //[Authorize(Roles = "Parent, Admin")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<VaccineHistoryResponseDTO>> UpdateVaccineHistory(string id, [FromBody] UpdateVaccineHistoryRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _vaccineHistoryService.UpdateVaccineHistoryAsync(id, request);
            if (response == null)
                return NotFound("Không tìm thấy lịch sử vaccine để cập nhật.");

            return Ok(response);
        }

        //[Authorize(Roles = "Parent, Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteVaccineHistory(string id)
        {
            var success = await _vaccineHistoryService.DeleteVaccineHistoryAsync(id);
            if (!success)
                return NotFound("Không tìm thấy lịch sử vaccine để xóa.");

            return NoContent();
        }
    }
}

