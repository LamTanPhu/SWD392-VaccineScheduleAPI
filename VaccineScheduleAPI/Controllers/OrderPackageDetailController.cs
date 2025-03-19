//using IServices.Interfaces.Orders;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using ModelViews.Requests.Order;
//using ModelViews.Responses.Order;

//namespace VaccineScheduleAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class OrderPackageDetailsController : ControllerBase
//    {
//        private readonly IOrderPackageDetailsService _service;

//        public OrderPackageDetailsController(IOrderPackageDetailsService service)
//        {
//            _service = service;
//        }

//        [Authorize(Roles = "Admin, Parent")]
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<OrderPackageDetailsResponseDTO>>> GetAll()
//        {
//            return Ok(await _service.GetAllOrderPackageDetailsAsync());
//        }

//        [Authorize(Roles = "Admin, Parent")]
//        [HttpGet("{id}")]
//        public async Task<ActionResult<OrderPackageDetailsResponseDTO>> GetById(string id)
//        {
//            var details = await _service.GetOrderPackageDetailsByIdAsync(id);
//            if (details == null) return NotFound();
//            return Ok(details);
//        }

//        [Authorize(Roles = "Admin, Parent")]
//        [HttpPost]
//        public async Task<ActionResult> Create(OrderPackageDetailsRequestDTO detailsDto)
//        {
//            await _service.AddOrderPackageDetailsAsync(detailsDto);
//            return CreatedAtAction(nameof(GetById), new { id = detailsDto.OrderId }, detailsDto);
//        }

//        [Authorize(Roles = "Admin, Parent")]
//        [HttpPut("{id}")]
//        public async Task<ActionResult> Update(string id, OrderPackageDetailsRequestDTO detailsDto)
//        {
//            await _service.UpdateOrderPackageDetailsAsync(id, detailsDto);
//            return NoContent();
//        }

//        [Authorize(Roles = "Admin, Parent")]
//        [HttpDelete("{id}")]
//        public async Task<ActionResult> Delete(string id)
//        {
//            await _service.DeleteOrderPackageDetailsAsync(id);
//            return NoContent();
//        }
//    }
//}
