using IServices.Interfaces.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.Order;
using ModelViews.Responses.Order;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin, Parent")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponseDTO>>> GetAll()
        {
            return Ok(await _service.GetAllOrdersAsync());
        }

        [Authorize(Roles = "Admin, Parent")]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDTO>> GetById(string id)
        {
            var order = await _service.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [Authorize(Roles = "Admin, Parent")]
        [HttpPost]
        public async Task<ActionResult> Create(OrderRequestDTO orderDto)
        {
            await _service.AddOrderAsync(orderDto);
            return CreatedAtAction(nameof(GetById), new { id = orderDto.ProfileId }, orderDto);
        }

        [Authorize(Roles = "Admin, Parent")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, OrderRequestDTO orderDto)
        {
            await _service.UpdateOrderAsync(id, orderDto);
            return NoContent();
        }

        [Authorize(Roles = "Admin, Parent")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _service.DeleteOrderAsync(id);
            return NoContent();
        }
    }
}
