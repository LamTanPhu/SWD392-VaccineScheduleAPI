using IServices.Interfaces.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.Order;
using ModelViews.Responses.Order;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderVaccineDetailsController : ControllerBase
    {
        private readonly IOrderVaccineDetailsService _service;

        public OrderVaccineDetailsController(IOrderVaccineDetailsService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderVaccineDetailsResponseDTO>>> GetAll()
        {
            return Ok(await _service.GetAllOrderVaccineDetailsAsync());
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderVaccineDetailsResponseDTO>> GetById(string id)
        {
            var details = await _service.GetOrderVaccineDetailsByIdAsync(id);
            if (details == null) return NotFound();
            return Ok(details);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<ActionResult> Create(OrderVaccineDetailsRequestDTO detailsDto)
        {
            await _service.AddOrderVaccineDetailsAsync(detailsDto);
            return CreatedAtAction(nameof(GetById), new { id = detailsDto.OrderId }, detailsDto);
        }

        [Authorize(Roles = "User")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, OrderVaccineDetailsRequestDTO detailsDto)
        {
            await _service.UpdateOrderVaccineDetailsAsync(id, detailsDto);
            return NoContent();
        }

        [Authorize(Roles = "User")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _service.DeleteOrderVaccineDetailsAsync(id);
            return NoContent();
        }
    }
}
