using IServices.Interfaces.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.Order;
using ModelViews.Requests.Payment;
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

        [HttpGet("by-parent/{parentId}")]
        [Authorize(Roles = "Parent, Admin")]
        public async Task<ActionResult<IEnumerable<OrderResponseDTO>>> GetOrdersByParentId(string parentId)
        {
            try
            {
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                if (userRole == "Parent" && currentUserId != parentId)
                    return Forbid("You can only view orders of your own children.");

                var orders = await _orderService.GetOrdersByParentIdAsync(parentId);
                return Ok(orders);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
            }
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

        [Authorize(Roles = "Admin, Parent")]
        [HttpPost("details/remove")]
        public async Task<ActionResult<OrderResponseDTO>> RemoveOrderDetails([FromBody] RemoveOrderDetailsRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedOrder = await _orderService.RemoveOrderDetailsAsync(request);
            return Ok(updatedOrder);
        }

        [Authorize(Roles = "Admin, Parent")]
        [HttpPost("pay-later")]
        public async Task<ActionResult<OrderResponseDTO>> SetPayLater([FromBody] PayLaterRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedOrder = await _orderService.SetPayLaterAsync(request);
            return Ok(updatedOrder);
        }

        [HttpGet("paid-by-parent/{parentId}")]
        [Authorize(Roles = "Parent, Admin")]
        public async Task<ActionResult<IEnumerable<OrderResponseDTO>>> GetPaidOrdersByParentId(string parentId)
        {
            try
            {
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                if (userRole == "Parent" && currentUserId != parentId)
                    return Forbid("You can only view your own paid orders.");

                var orders = await _orderService.GetPaidOrdersByParentIdAsync(parentId);
                return Ok(orders);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
            }
        }


    }
}
