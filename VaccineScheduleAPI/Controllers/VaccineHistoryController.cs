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

        [HttpPost("sendDocument")]
        [Authorize(Roles = "Parent, Admin")] // Giới hạn quyền cho Staff và Admin
        public async Task<ActionResult<VaccineHistoryResponseDTO>> SendVaccineDocument([FromBody] CreateVaccineHistoryRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Invalid request data.", Errors = ModelState });
            var response = await _vaccineHistoryService.AddVaccineHistoryAsync(request);
            return Ok(response);

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

