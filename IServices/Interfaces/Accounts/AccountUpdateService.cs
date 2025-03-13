using IRepositories.IRepository.Accounts;
using IRepositories.IRepository.Inventory;
using IRepositories.IRepository;
using ModelViews.Requests.Auth;
using ModelViews.Responses.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Accounts
{
    public class AccountUpdateService : IAccountUpdateService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IVaccineCenterRepository _vaccineCenterRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProfileService _userProfileService;

        public AccountUpdateService(
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

        public async Task<ProfileResponseDTO> UpdateAccountAsync(string username, UpdateAccountRequestDTO request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var account = await _accountRepository.GetByUsernameAsync(username);
                if (account == null || account.DeletedTime != null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw new Exception("Account not found or has been deleted.");
                }

                // Update account fields
                account.Username = request.Username ?? account.Username;
                account.Email = request.Email ?? account.Email;
                account.Status = request.Status ?? account.Status;

                // Handle VaccineCenter update
                if (request.VaccineCenterId != null)
                {
                    var vaccineCenter = await _vaccineCenterRepository.GetByIdAsync(request.VaccineCenterId);
                    if (vaccineCenter == null || vaccineCenter.DeletedTime != null)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        throw new Exception("Vaccine center not found or has been deleted.");
                    }
                    account.VaccineCenterId = request.VaccineCenterId;
                }
                else
                {
                    account.VaccineCenterId = null; // Allow clearing the assignment
                }

                await _accountRepository.UpdateAsync(account);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Return updated profile
                var updatedProfile = await _userProfileService.GetProfileByUsernameAsync(account.Username);
                if (updatedProfile == null)
                {
                    throw new Exception("Failed to retrieve updated profile.");
                }

                return updatedProfile;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw; // Let the controller handle the exception
            }
        }
    }
}
