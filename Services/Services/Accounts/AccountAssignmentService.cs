using IRepositories.IRepository;
using IRepositories.IRepository.Accounts;
using IRepositories.IRepository.Inventory;
using IServices.Interfaces.Accounts;
using ModelViews.Requests.Auth;
using ModelViews.Responses.Auth;
using Repositories.Repository;
using System;
using System.Threading.Tasks;

namespace Services.Services.Accounts
{
    public class AccountAssignmentService : IAccountAssignmentService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IVaccineCenterRepository _vaccineCenterRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProfileService _userProfileService;

        public AccountAssignmentService(
            IAccountRepository accountRepository,
            IVaccineCenterRepository vaccineCenterRepository,
            IUnitOfWork unitOfWork,
            IUserProfileService userProfileService)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _vaccineCenterRepository = vaccineCenterRepository ?? throw new ArgumentNullException(nameof(vaccineCenterRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userProfileService = userProfileService ?? throw new ArgumentNullException(nameof(userProfileService));
        }

        public async Task<AssignAccountToVaccineCenterResponseDTO> AssignAccountToVaccineCenterAsync(AssignAccountToVaccineCenterRequestDTO request)
        {
            // Validate input
            if (string.IsNullOrEmpty(request.AccountId) || string.IsNullOrEmpty(request.VaccineCenterId))
            {
                return new AssignAccountToVaccineCenterResponseDTO
                {
                    Success = false,
                    Message = "AccountId and VaccineCenterId are required.",
                    Account = null
                };
            }

            try
            {
                // Begin transaction
                await _unitOfWork.BeginTransactionAsync();

                // Fetch account
                var account = await _accountRepository.GetByIdAsync(request.AccountId);
                if (account == null || account.DeletedTime != null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new AssignAccountToVaccineCenterResponseDTO
                    {
                        Success = false,
                        Message = "Account not found or has been deleted.",
                        Account = null
                    };
                }

                // Fetch vaccine center
                var vaccineCenter = await _vaccineCenterRepository.GetByIdAsync(request.VaccineCenterId);
                if (vaccineCenter == null || vaccineCenter.DeletedTime != null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new AssignAccountToVaccineCenterResponseDTO
                    {
                        Success = false,
                        Message = "Vaccine center not found or has been deleted.",
                        Account = null
                    };
                }

                // Assign vaccine center to account
                account.VaccineCenterId = request.VaccineCenterId;

                // Update account
                await _accountRepository.UpdateAsync(account);
                await _unitOfWork.SaveAsync();

                // Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                // Fetch updated profile using UserProfileService
                var updatedProfile = await _userProfileService.GetProfileByUsernameAsync(account.Username);
                if (updatedProfile == null)
                {
                    return new AssignAccountToVaccineCenterResponseDTO
                    {
                        Success = false,
                        Message = "Failed to retrieve updated profile.",
                        Account = null
                    };
                }

                // Return success response
                return new AssignAccountToVaccineCenterResponseDTO
                {
                    Success = true,
                    Message = "Account successfully assigned to vaccine center.",
                    Account = updatedProfile
                };
            }
            catch (Exception ex)
            {
                // Rollback on error
                await _unitOfWork.RollbackTransactionAsync();
                return new AssignAccountToVaccineCenterResponseDTO
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Account = null
                };
            }
        }
    }
}