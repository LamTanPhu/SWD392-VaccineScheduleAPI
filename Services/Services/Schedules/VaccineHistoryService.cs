using IRepositories.Entity.Schedules;
using IRepositories.IRepository.Accounts;
using IRepositories.IRepository.Vaccines;
using IRepositories.IRepository;
using ModelViews.Requests.VaccineHistory;
using ModelViews.Responses.VaccineHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IServices.Interfaces.Schedules;
using IRepositories.IRepository.Schedules;
using Repositories.Repository.Schedules;

namespace Services.Services.Schedules
{
    public class VaccineHistoryService : IVaccineHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVaccineRepository _vaccineRepository;
        private readonly IChildrenProfileRepository _childrenProfileRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IVaccineHistoryRepository _vaccineHistoryRepository;

        public VaccineHistoryService(IUnitOfWork unitOfWork,
                                     IVaccineRepository vaccineRepository,
                                     IChildrenProfileRepository childrenProfileRepository,
                                     IAccountRepository accountRepository,
                                     IVaccineHistoryRepository vaccineHistoryRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _vaccineRepository = vaccineRepository ?? throw new ArgumentNullException(nameof(vaccineRepository));
            _childrenProfileRepository = childrenProfileRepository ?? throw new ArgumentNullException(nameof(childrenProfileRepository));
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _vaccineHistoryRepository = vaccineHistoryRepository ?? throw new ArgumentNullException(nameof(vaccineHistoryRepository));
        }

        public async Task<CreateVaccineHistoryResponseDTO> AddVaccineHistoryAsync(AddVaccineHistoryRequestDTO request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");

            if (string.IsNullOrEmpty(request.ProfileId))
                throw new ArgumentException("ProfileId is required.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Kiểm tra ProfileId có tồn tại không
                var profile = await _childrenProfileRepository.GetByIdAsync(request.ProfileId);
                if (profile == null || profile.DeletedTime != null)
                    throw new Exception("Children profile not found or has been deleted.");

                // Tạo mới VaccineHistory
                var vaccineHistory = new VaccineHistory
                {
                    Id = Guid.NewGuid().ToString(),
                    ProfileId = request.ProfileId,
                    DocumentationProvided = request.DocumentationProvided,
                    Notes = request.Notes,
                    VerifiedStatus = request.VerifiedStatus,
                    VaccineId = null, 
                    AccountId = profile.AccountId, 
                    CenterId = null, 
                    AdministeredDate = DateTime.Now,
                    AdministeredBy = "Unknow", 
                    VaccinedStatus = 0, 
                    DosedNumber = 0, 
                };

                await _vaccineHistoryRepository.InsertAsync(vaccineHistory);
                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitTransactionAsync();

                // Trả về response
                return new CreateVaccineHistoryResponseDTO
                {
                    Id = vaccineHistory.Id,
                    ProfileId = vaccineHistory.ProfileId,
                    DocumentationProvided = vaccineHistory.DocumentationProvided,
                    Notes = vaccineHistory.Notes,
                    VerifiedStatus = vaccineHistory.VerifiedStatus,
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to add vaccine history: {ex.Message}");
            }
        }

        //---------------------------------------------------------------------------------
        public async Task<VaccineHistoryResponseDTO> GetVaccineHistoryByIdAsync(string id)
        {
            var history = await _unitOfWork.GetRepository<VaccineHistory>().GetByIdAsync(id);
            if (history == null)
                throw new Exception("Lịch sử vaccine không tồn tại.");

            return MapToResponseDTO(history);
        }

        public async Task<IEnumerable<VaccineHistoryResponseDTO>> GetAllVaccineHistoriesAsync()
        {
            var histories = await _unitOfWork.GetRepository<VaccineHistory>().GetAllAsync();
            return histories.Select(MapToResponseDTO);
        }

        public async Task<VaccineHistoryResponseDTO> UpdateVaccineHistoryAsync(string id, UpdateVaccineHistoryRequestDTO request)
        {
            var history = await _unitOfWork.GetRepository<VaccineHistory>().GetByIdAsync(id);
            if (history == null)
                throw new Exception("Lịch sử vaccine không tồn tại.");

            history.AdministeredDate = request.AdministeredDate;
            history.AdministeredBy = request.AdministeredBy;
            history.DocumentationProvided = request.DocumentationProvided.ToString(); 
            history.Notes = request.Notes;
            history.VerifiedStatus = request.VerifiedStatus;
            history.VaccinedStatus = request.VaccinedStatus;
            history.DosedNumber = request.DosedNumber;
            history.LastUpdatedTime = DateTime.Now;

            _unitOfWork.GetRepository<VaccineHistory>().Update(history);
            await _unitOfWork.SaveAsync();

            return MapToResponseDTO(history);
        }

        public async Task<bool> DeleteVaccineHistoryAsync(string id)
        {
            var history = await _unitOfWork.GetRepository<VaccineHistory>().GetByIdAsync(id);
            if (history == null)
                throw new Exception("Lịch sử vaccine không tồn tại.");

            _unitOfWork.GetRepository<VaccineHistory>().Delete(history);
            await _unitOfWork.SaveAsync();
            return true;
        }
        //---------------------------------------------------------------------------------
        private VaccineHistoryResponseDTO MapToResponseDTO(VaccineHistory entity)
        {
            return new VaccineHistoryResponseDTO
            {
                Id = entity.Id,
                VaccineId = entity.VaccineId,
                ProfileId = entity.ProfileId,
                AccountId = entity.AccountId,
                CenterId = entity.CenterId,
                AdministeredDate = entity.AdministeredDate,
                AdministeredBy = entity.AdministeredBy,
                DocumentationProvided = entity.DocumentationProvided,
                Notes = entity.Notes,
                VerifiedStatus = entity.VerifiedStatus,
                VaccinedStatus = entity.VaccinedStatus,
                DosedNumber = entity.DosedNumber
            };
        }
    }
}
