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

namespace Services.Services.Schedules
{
    public class VaccineHistoryService : IVaccineHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVaccineRepository _vaccineRepository;
        private readonly IChildrenProfileRepository _childrenProfileRepository;
        private readonly IAccountRepository _accountRepository;

        public VaccineHistoryService(IUnitOfWork unitOfWork,
                                     IVaccineRepository vaccineRepository,
                                     IChildrenProfileRepository childrenProfileRepository,
                                     IAccountRepository accountRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _vaccineRepository = vaccineRepository ?? throw new ArgumentNullException(nameof(vaccineRepository));
            _childrenProfileRepository = childrenProfileRepository ?? throw new ArgumentNullException(nameof(childrenProfileRepository));
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        }

        public async Task<VaccineHistoryResponseDTO> CreateVaccineHistoryAsync(CreateVaccineHistoryRequestDTO request, string accountId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {

                var profile = await _childrenProfileRepository.GetByIdAsync(request.ProfileId);
                if (profile == null || profile.AccountId != accountId)
                    throw new Exception("ChildrenProfile không tồn tại hoặc không thuộc tài khoản này.");

                var account = await _accountRepository.GetByIdAsync(accountId);
                if (account == null || account.Role != IRepositories.Enum.RoleEnum.Parent)
                    throw new Exception("Tài khoản không hợp lệ hoặc không phải Parent.");

                // Tạo VaccineHistory
                var vaccineHistory = new VaccineHistory
                {
                    Id = Guid.NewGuid().ToString(),
                    ProfileId = request.ProfileId,
                    AccountId = accountId,
                    CenterId = null, // Không thuộc VaccineCenter trong hệ thống
                    AdministeredDate = request.AdministeredDate,
                    AdministeredBy = request.AdministeredBy,
                    DocumentationProvided = request.DocumentationProvided,
                    Notes = request.Notes,
                    VerifiedStatus = 0, // Chưa duyệt
                    VaccinedStatus = 1, // Đã tiêm (giả định)
                    DosedNumber = request.DosedNumber,
                    CreatedTime = DateTime.Now,
                    LastUpdatedTime = DateTime.Now
                };

                await _unitOfWork.GetRepository<VaccineHistory>().InsertAsync(vaccineHistory);
                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitTransactionAsync();

                return MapToResponseDTO(vaccineHistory);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Tạo VaccineHistory thất bại: {ex.Message}");
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
