using IServices.Interfaces.Schedules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.Feedback;
using ModelViews.Responses.Feedback;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _service;

        public FeedbackController(IFeedbackService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin, Parent")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FeedbackResponseDTO>>> GetAll()
        {
            return Ok(await _service.GetAllFeedbacksAsync());
        }

        [Authorize(Roles = "Admin, Parent")]
        [HttpGet("{id}")]
        public async Task<ActionResult<FeedbackResponseDTO>> GetById(string id)
        {
            var feedback = await _service.GetFeedbackByIdAsync(id);
            if (feedback == null) return NotFound();
            return Ok(feedback);
        }

        [Authorize(Roles = "Parent")]
        [HttpPost]
        public async Task<ActionResult> Create(FeedbackRequestDTO feedbackDto)
        {
            await _service.AddFeedbackAsync(feedbackDto);
            return CreatedAtAction(nameof(GetById), new { id = feedbackDto.OrderId }, feedbackDto);
        }

        [Authorize(Roles = "Parent")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, FeedbackRequestDTO feedbackDto)
        {
            await _service.UpdateFeedbackAsync(id, feedbackDto);
            return NoContent();
        }

        [Authorize(Roles = "Parent")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _service.DeleteFeedbackAsync(id);
            return NoContent();
        }
    }
}
