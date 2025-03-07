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

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public PaymentsController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDetailsResponseDTO>>> GetAll()
        {
            var payments = await _context.Payments.Include(p => p.Order).ToListAsync();

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
        public async Task<ActionResult<PaymentDetailsResponseDTO>> GetById(string id)
        {
            var payment = await _context.Payments.Include(p => p.Order).FirstOrDefaultAsync(m => m.Id == id);
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

        [HttpPost]
        public async Task<ActionResult<PaymentDetailsResponseDTO>> Create([FromBody] PaymentDetailsRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var payment = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                OrderId = request.OrderId,
                PaymentName = request.PaymentName,
                PaymentMethod = request.PaymentMethod,
                PaymentDate = request.PaymentDate,
                PaymentStatus = request.PaymentStatus,
                PayAmount = request.PayAmount
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var response = new PaymentDetailsResponseDTO
            {
                OrderId = payment.OrderId,
                PaymentName = payment.PaymentName,
                PaymentMethod = payment.PaymentMethod,
                PaymentDate = payment.PaymentDate,
                PaymentStatus = payment.PaymentStatus,
                PayAmount = payment.PayAmount
            };

            return CreatedAtAction(nameof(GetById), new { id = payment.Id }, response);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool PaymentExists(string id)
        {
            return _context.Payments.Any(e => e.Id == id);
        }
    }
}
