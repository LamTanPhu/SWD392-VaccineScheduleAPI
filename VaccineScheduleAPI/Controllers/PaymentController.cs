using IServices.Interfaces.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.VNPay;
using ModelViews.Responses.VNPay;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVNPayService _vnPayService;

        public PaymentController(IVNPayService vnPayService)
        {
            _vnPayService = vnPayService ?? throw new ArgumentNullException(nameof(vnPayService));
        }

        [AllowAnonymous]
        [HttpPost("vnpay")]
        public async Task<ActionResult<VNPayPaymentResponseDTO>> CreateVNPayPayment([FromBody] VNPayPaymentRequestDTO request)
        {
            var response = await _vnPayService.CreatePaymentUrlAsync(request);
            return Ok(response);
        }

        [AllowAnonymous] // VNPay callback không cần auth
        [HttpGet("vnpay-return")]
        public async Task<ActionResult<VNPayReturnResponseDTO>> HandleVNPayReturn()
        {
            var query = HttpContext.Request.Query;
            var response = await _vnPayService.HandlePaymentReturnAsync(query);
            return Ok(response);
        }
    }

}
