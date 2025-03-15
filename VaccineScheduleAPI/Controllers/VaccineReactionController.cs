using IServices.Interfaces.Schedules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.Schedule;
using ModelViews.Responses.Schedule;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaccineReactionController : ControllerBase
    {
        private readonly IVaccineReactionService _service;

        public VaccineReactionController(IVaccineReactionService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin, Parent")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VaccineReactionResponseDTO>>> GetAll()
        {
            return Ok(await _service.GetAllReactionsAsync());
        }

        [Authorize(Roles = "Admin, Parent")]
        [HttpGet("{id}")]
        public async Task<ActionResult<VaccineReactionResponseDTO>> GetById(string id)
        {
            var reaction = await _service.GetReactionByIdAsync(id);
            if (reaction == null) return NotFound();
            return Ok(reaction);
        }

        [Authorize(Roles = "Parent")]
        [HttpPost]
        public async Task<ActionResult> Create(VaccineReactionRequestDTO requestDto)
        {
            var createdReaction = await _service.CreateReactionAsync(requestDto);
            return CreatedAtAction(nameof(GetById), new { id = createdReaction.Id }, createdReaction);
        }

        [Authorize(Roles = "Parent")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, VaccineReactionRequestDTO requestDto)
        {
            await _service.UpdateReactionAsync(id, requestDto);
            return NoContent();
        }

        [Authorize(Roles = "Parent")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _service.DeleteReactionAsync(id);
            return NoContent();
        }
    }
}
