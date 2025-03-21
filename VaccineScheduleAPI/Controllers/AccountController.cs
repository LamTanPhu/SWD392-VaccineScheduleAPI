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
        private readonly IAccountService _accountService;

        public AccountController(IAccountAssignmentService accountAssignmentService, IAccountService accountService)
        {
            _accountAssignmentService = accountAssignmentService ?? throw new ArgumentNullException(nameof(accountAssignmentService));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<AccountResponseDTO>>> GetAllAccounts()
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AccountResponseDTO>> GetAccountById(string id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null)
                return NotFound();
            return Ok(account);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var success = await _accountService.SoftDeleteAccountAsync(id);
            if (!success)
                return NotFound();
            return NoContent();
        }

        [HttpPost("assign-to-vaccine-center")]
        [Authorize(Roles = "Admin")]
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