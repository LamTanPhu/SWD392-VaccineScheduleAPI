using AutoMapper;
using IRepositories.Entity.Accounts;
using IRepositories.IRepository;
using IRepositories.IRepository.Accounts;
using IServices.Interfaces.Accounts;
using ModelViews.Requests.ChildrenProfile;
using ModelViews.Responses.ChildrenProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services.Accounts
{
    public class ChildrenProfileService : IChildrenProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChildrenProfileRepository _childrenProfileRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public ChildrenProfileService(
            IUnitOfWork unitOfWork,
            IChildrenProfileRepository childrenProfileRepository,
            IAccountRepository accountRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _childrenProfileRepository = childrenProfileRepository ?? throw new ArgumentNullException(nameof(childrenProfileRepository));
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<ChildrenProfileResponseDTO>> GetMyChildrenProfilesAsync(string userEmail)
        {
            var account = await _accountRepository.GetByEmailAsync(userEmail);
            if (account == null)
                throw new Exception("User not found.");

            var profiles = await _childrenProfileRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ChildrenProfileResponseDTO>>(
                profiles.Where(p => p.AccountId == account.Id && p.Status != "0"));
        }

        public async Task<ChildrenProfileResponseDTO> CreateProfileAsync(string userEmail, ChildrenProfileCreateUpdateDTO profileDto)
        {
            var account = await _accountRepository.GetByEmailAsync(userEmail);
            if (account == null)
                throw new Exception("User not found.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var profile = _mapper.Map<ChildrenProfile>(profileDto);
                profile.AccountId = account.Id;

                await _childrenProfileRepository.InsertAsync(profile);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<ChildrenProfileResponseDTO>(profile);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to create children profile: " + ex.Message, ex);
            }
        }

        public async Task UpdateProfileAsync(string id, string userEmail, ChildrenProfileCreateUpdateDTO profileDto)
        {
            var account = await _accountRepository.GetByEmailAsync(userEmail);
            if (account == null)
                throw new Exception("User not found.");

            var profile = await _childrenProfileRepository.GetByIdAsync(id);
            if (profile == null || profile.Status == "0")
                throw new Exception("Children profile not found.");

            if (profile.AccountId != account.Id)
                throw new Exception("You can only update your own children's profiles.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _mapper.Map(profileDto, profile);

                await _childrenProfileRepository.UpdateAsync(profile);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to update children profile: " + ex.Message, ex);
            }
        }

        public async Task DeleteProfileAsync(string id, string userEmail)
        {
            var account = await _accountRepository.GetByEmailAsync(userEmail);
            if (account == null)
                throw new Exception("User not found.");

            var profile = await _childrenProfileRepository.GetByIdAsync(id);
            if (profile == null || profile.Status == "0")
                throw new Exception("Children profile not found.");

            if (profile.AccountId != account.Id)
                throw new Exception("You can only delete your own children's profiles.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                profile.Status = "0"; // Soft delete
                await _childrenProfileRepository.UpdateAsync(profile);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to delete children profile: " + ex.Message, ex);
            }
        }
    }
}