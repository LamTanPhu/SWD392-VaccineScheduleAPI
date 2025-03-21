using IServices.Interfaces.Schedules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.History;
using ModelViews.Requests.VaccineHistory;
using ModelViews.Responses.VaccineHistory;
using System.Security.Claims;

namespace VaccineScheduleAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class VaccineHistoryController : ControllerBase
    {
        private readonly IVaccineHistoryService _service;

        public VaccineHistoryController(IVaccineHistoryService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VaccineHistoryResponseDTO>>> GetAll()
        {
            var histories = await _service.GetAllVaccineHistoriesAsync();
            return Ok(histories);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<VaccineHistoryResponseDTO>> GetById(string id)
        {
            var history = await _service.GetVaccineHistoryByIdAsync(id);
            if (history == null) return NotFound();
            return Ok(history);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<VaccineHistoryResponseDTO>> Create([FromBody] CreateVaccineHistoryRequestDTO vaccineHistoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdHistory = await _service.AddVaccineHistoryAsync(vaccineHistoryDto);
            return CreatedAtAction(nameof(GetById), new { id = createdHistory.Id }, createdHistory);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<VaccineHistoryResponseDTO>> Update(string id, [FromBody] CreateVaccineHistoryRequestDTO vaccineHistoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedHistory = await _service.UpdateVaccineHistoryAsync(id, vaccineHistoryDto);
            if (updatedHistory == null)
                return NotFound();

            return Ok(updatedHistory);
        }

        [Authorize(Roles = "Admin, Parent")]
        [HttpPost("send-certificate")]
        public async Task<ActionResult<VaccineHistoryResponseDTO>> SendVaccineCertificate([FromForm] SendVaccineCertificateRequestDTO certificateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdCertificate = await _service.SendVaccineCertificateAsync(certificateDto);
            return CreatedAtAction(nameof(GetById), new { id = createdCertificate.Id }, createdCertificate);
        }

        [Authorize(Roles = "Admin, Staff")] // Chỉ Admin hoặc Staff xem được danh sách chưa xác thực
        [HttpGet("pending-certificates")]
        public async Task<ActionResult<IEnumerable<VaccineHistoryResponseDTO>>> GetPendingCertificates()
        {
            var pendingCertificates = await _service.GetPendingCertificatesAsync();
            return Ok(pendingCertificates);
        }

        [Authorize(Roles = "Admin, Staff")] // Chỉ Admin hoặc Staff xác thực được
        [HttpPut("verify-certificate/{id}")]
        public async Task<ActionResult<VaccineHistoryResponseDTO>> VerifyCertificate(string id, [FromQuery] bool isAccepted, [FromBody] CreateVaccineHistoryRequestDTO vaccineHistoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var verifiedHistory = await _service.VerifyCertificateAsync(id, vaccineHistoryDto, isAccepted);
            if (verifiedHistory == null)
                return NotFound();

            return Ok(verifiedHistory);
        }
    }


}

