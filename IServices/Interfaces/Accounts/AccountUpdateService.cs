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


        public async Task<ProfileResponseDTO> UpdateAccountAsync(string userName, UpdateAccountRequestDTO request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var account = await _accountRepository.GetByUsernameAsync(userName);
                if (account == null)
                    throw new Exception("Tài khoản không tồn tại.");

                // Cập nhật các trường từ request
                account.Username = request.Username ?? account.Username; // Giữ nguyên nếu null
                account.PhoneNumber = request.PhoneNumber;
                account.ImageProfile = request.ImageProfile;
                account.LastUpdatedTime = DateTime.Now; // Cập nhật thời gian chỉnh sửa

                await _accountRepository.UpdateAsync(account);
                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitTransactionAsync();

                // Trả về thông tin tài khoản đã cập nhật
                return new ProfileResponseDTO
                {
                    AccountId = account.Id,
                    Username = account.Username,
                    Email = account.Email,
                    PhoneNumber = account.PhoneNumber,
                    ImageProfile = account.ImageProfile,
                    Role = account.Role.ToString(),
                    Status = account.Status
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Cập nhật tài khoản thất bại: {ex.Message}");
            }
        }

    }
}
