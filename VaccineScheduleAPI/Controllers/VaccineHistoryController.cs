
﻿using IServices.Interfaces.Schedules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.VaccineHistory;
using ModelViews.Responses.VaccineHistory;

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


    }
}
