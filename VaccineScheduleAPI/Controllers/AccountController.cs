using IServices.Interfaces.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.Auth;
using ModelViews.Responses.Auth;
using System.Threading.Tasks;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountAssignmentService _accountAssignmentService;

        public AccountController(IAccountAssignmentService accountAssignmentService)
        {
            _accountAssignmentService = accountAssignmentService;
        }

        [HttpPost("assign-to-vaccine-center")]
        //[Authorize(Roles = "Admin")]
        // Maybe Admin is the only one allowed to use this, commented out authorize for testing
        public async Task<IActionResult> AssignAccountToVaccineCenter([FromBody] AssignAccountToVaccineCenterRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _accountAssignmentService.AssignAccountToVaccineCenterAsync(request);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }


    }
}