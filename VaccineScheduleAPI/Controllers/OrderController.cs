using IServices.Interfaces.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.Order;
using ModelViews.Responses.Order;
using Services.Services.Orders;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly IOrderService _orderService;

        public OrderController(IOrderService service)
        {
            _orderService = service;
        }

        [Authorize(Roles = "Admin, Parent")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponseDTO>>> GetOrderAll()
        {
            return Ok(await _orderService.GetAllOrdersAsync());
        }

        [Authorize(Roles = "Admin, Parent")]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDTO>> GetOrderById(string id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [Authorize(Roles = "Admin, Parent")]
        [HttpPost]
        public async Task<ActionResult<OrderResponseDTO>> CreateOrder([FromBody] OrderRequestDTO orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _orderService.CreateOrderAsync(orderDto);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }

        [Authorize(Roles = "Admin, Parent")]
        [HttpPost("details/add")]
        public async Task<ActionResult<OrderResponseDTO>> AddOrderDetails([FromBody] AddOrderDetailsRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedOrder = await _orderService.AddOrderDetailsAsync(request);
            return Ok(updatedOrder);
        }



    }
}
