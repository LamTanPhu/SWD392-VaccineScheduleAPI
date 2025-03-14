using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IRepositories.Entity.Orders;
using Repositories.Context;
using ModelViews.Responses.Order;
using ModelViews.Requests.Order;
using Microsoft.AspNetCore.Authorization;
using ModelViews.Requests.VNPay;
using ModelViews.Responses.VNPay;
using IServices.Interfaces.Orders;
using ModelViews.Responses.Payment;
using Services.Services.Orders;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDetailsResponseDTO>>> GetAll()
        {
            var payments = await _paymentService.GetAllPaymentDetailsAsync();

            var response = payments.Select(p => new PaymentDetailsResponseDTO
            {
                OrderId = p.OrderId,
                PaymentName = p.PaymentName,
                PaymentMethod = p.PaymentMethod,
                PaymentDate = p.PaymentDate,
                PaymentStatus = p.PaymentStatus,
                PayAmount = p.PayAmount
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDetailsResponseDTO>> GetByName(string name)
        {
            var payment = await _paymentService.GetPaymentDetailsByNameAsync(name);
            if (payment == null)
            {
                return NotFound();
            }

            var response = new PaymentDetailsResponseDTO
            {
                OrderId = payment.OrderId,
                PaymentName = payment.PaymentName,
                PaymentMethod = payment.PaymentMethod,
                PaymentDate = payment.PaymentDate,
                PaymentStatus = payment.PaymentStatus,
                PayAmount = payment.PayAmount
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDetailsResponseDTO>> GetById(string id)
        {
            var payment = await _paymentService.GetPaymentDetailsByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            var response = new PaymentDetailsResponseDTO
            {
                OrderId = payment.OrderId,
                PaymentName = payment.PaymentName,
                PaymentMethod = payment.PaymentMethod,
                PaymentDate = payment.PaymentDate,
                PaymentStatus = payment.PaymentStatus,
                PayAmount = payment.PayAmount
            };

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PaymentDetailsRequestDTO request)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            payment.OrderId = request.OrderId;
            payment.PaymentName = request.PaymentName;
            payment.PaymentMethod = request.PaymentMethod;
            payment.PaymentDate = request.PaymentDate;
            payment.PaymentStatus = request.PaymentStatus;
            payment.PayAmount = request.PayAmount;

            _context.Entry(payment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        private bool PaymentExists(string id)
        {
            return _context.Payments.Any(e => e.Id == id);
        }

        [AllowAnonymous]
        [HttpPost("vnpay")]
        public async Task<ActionResult<VNPayPaymentResponseDTO>> CreateVNPayPayment([FromBody] VNPayPaymentRequestDTO request)
        {
            var response = await _paymentService.CreatePaymentUrlAsync(request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("vnpay-return")]
        public async Task<ActionResult<VNPayReturnResponseDTO>> HandleVNPayReturn()
        {
            var query = HttpContext.Request.Query;
            var response = await _paymentService.HandlePaymentReturnAsync(query);
            return Ok(response);
        }


        [AllowAnonymous]
        [HttpPost("vnpay-qr")]
        [Produces("image/png")] // Chỉ định response là hình ảnh PNG
        public async Task<IActionResult> CreateVNPayQRCode([FromBody] VNPayPaymentRequestDTO request)
        {
            var qrCodeBytes = await _paymentService.CreateQRCodeAsync(request);
            return File(qrCodeBytes, "image/png", "qrcode.png");
        }

        [Authorize(Roles = "Admin")] // Chỉ Admin tại cơ sở được phép thanh toán
        [HttpPost("cash")]
        public async Task<ActionResult<PaymentDetailsResponseDTO>> PayAtFacility([FromBody] PayAtFacilityRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedOrder = await _paymentService.PayAtFacilityAsync(request);
            return Ok(updatedOrder);
        }


    }
}
