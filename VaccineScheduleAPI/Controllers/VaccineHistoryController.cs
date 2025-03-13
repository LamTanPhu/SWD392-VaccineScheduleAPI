using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IServices.Interfaces.Schedules;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModelViews.Requests.History;
using ModelViews.Responses.History;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/vaccine-history")]
    [ApiController]
    public class VaccineHistoryController : ControllerBase
    {
        private readonly IVaccineHistoryService _service;

        public VaccineHistoryController(IVaccineHistoryService service)
        {
            _service = service;
        }

        // Lấy lịch sử tiêm của một phụ huynh (Parent)
        [Authorize(Roles = "Parent")]
        [HttpGet("parent/{parentId}")]
        public async Task<ActionResult<IEnumerable<VaccineHistoryResponseDTO>>> GetByParentId(string parentId)
        {
            return Ok(await _service.GetAllByUserIdAsync(parentId));
        }

        // Tìm lịch sử tiêm theo ngày cho phụ huynh
        [Authorize(Roles = "Parent")]
        [HttpGet("parent/search")]
        public async Task<ActionResult<IEnumerable<VaccineHistoryResponseDTO>>> SearchByDateForParent(
            [FromQuery] string parentId, [FromQuery] DateTime date)
        {
            return Ok(await _service.SearchByDateAsync(parentId, date));
        }

        // Nhân viên lấy toàn bộ lịch sử tiêm
        [Authorize(Roles = "Staff")]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<VaccineHistoryResponseDTO>>> GetAllHistories()
        {
            return Ok(await _service.GetAllAsync());
        }

        // Nhân viên tìm lịch sử tiêm theo trung tâm
        [Authorize(Roles = "Staff")]
        [HttpGet("search-by-center")]
        public async Task<ActionResult<IEnumerable<VaccineHistoryResponseDTO>>> SearchByCenter([FromQuery] string centerId)
        {
            return Ok(await _service.SearchByCenterIdAsync(centerId));
        }

        // Nhân viên tạo lịch sử tiêm mới
        [Authorize(Roles = "Staff")]
        [HttpPost("create")]
        public async Task<ActionResult> CreateHistory(VaccineHistoryRequestDTO requestDto)
        {
            await _service.CreateAsync(requestDto);
            return CreatedAtAction(nameof(GetByParentId), new { parentId = requestDto.AccountId }, requestDto);
        }

        // Nhân viên cập nhật lịch sử tiêm
        [Authorize(Roles = "Staff")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateHistory(string id, VaccineHistoryRequestDTO requestDto)
        {
            await _service.UpdateAsync(id, requestDto);
            return NoContent();
        }
    }
}
