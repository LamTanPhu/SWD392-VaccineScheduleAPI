using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ModelViews.Requests.Order;
using ModelViews.Requests.VNPay;
using ModelViews.Responses.Order;
using ModelViews.Responses.Payment;
using ModelViews.Responses.VNPay;
using IServices.Interfaces.Orders;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDetailsResponseDTO>>> GetAll()
        {
            var payments = await _paymentService.GetAllPaymentDetailsAsync();
            return Ok(payments);
        }

        [HttpGet("name")]
        public async Task<ActionResult<PaymentDetailsResponseDTO>> GetByName(string name)
        {
            var payment = await _paymentService.GetPaymentDetailsByNameAsync(name);
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
        }

        [HttpGet("id")]
        public async Task<ActionResult<PaymentDetailsResponseDTO>> GetById(string id)
        {
            var payment = await _paymentService.GetPaymentDetailsByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
        }

        [AllowAnonymous]
        [HttpPost("vnpay")]
        public async Task<ActionResult<VNPayPaymentResponseDTO>> CreateVNPayPayment([FromBody] VNPayPaymentRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _paymentService.CreatePaymentUrlAsync(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("vnpay-return")]
        public async Task<IActionResult> HandleVNPayReturn()
        {
            try
            {
                var query = HttpContext.Request.Query;
                var response = await _paymentService.HandlePaymentReturnAsync(query);
                //return Ok(response);
                // Redirect to the frontend schedule page
                return Redirect("http://localhost:5173/schedule");
            }
            catch (Exception ex)
            {
                //return StatusCode(500, $"Internal server error: {ex.Message}");
                // Optionally, you can log the error or handle it differently
                return Redirect("http://localhost:5173/schedule");
            }
        }

        [AllowAnonymous]
        [HttpPost("vnpay-qr")]
        [Produces("image/png")]
        public async Task<IActionResult> CreateVNPayQRCode([FromBody] VNPayPaymentRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var qrCodeBytes = await _paymentService.CreateQRCodeAsync(request);
                return File(qrCodeBytes, "image/png", "qrcode.png");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("cash")]
        public async Task<ActionResult<PaymentDetailsResponseDTO>> PayAtFacility([FromBody] PayAtFacilityRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var payment = await _paymentService.PayAtFacilityAsync(request);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}