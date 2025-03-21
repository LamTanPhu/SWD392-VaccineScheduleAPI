using AutoMapper;
using IRepositories.Entity.Accounts;
using IRepositories.Entity.Schedules;
using IRepositories.Entity.Vaccines;
using IRepositories.IRepository;
using IRepositories.IRepository.Accounts;
using IRepositories.IRepository.Schedules;
using IRepositories.IRepository.Vaccines;
using IServices.Interfaces.Schedules;
using IServices.Interfaces.Vaccines; // Thêm để dùng IImageUploadService
using ModelViews.Requests.History;
using ModelViews.Requests.VaccineHistory;
using ModelViews.Responses.VaccineHistory;
using Repositories.Repository.Accounts;
using Repositories.Repository.Schedules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services.Schedules
{
    public class VaccineHistoryService : IVaccineHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVaccineHistoryRepository _vaccineHistoryRepository;
        private readonly IChildrenProfileRepository _childrenProfileRepository;
        private readonly IImageUploadService _imageUploadService; // Thêm service upload ảnh
        private readonly IMapper _mapper;

        public VaccineHistoryService(
            IUnitOfWork unitOfWork,
            IVaccineHistoryRepository repository,
            IChildrenProfileRepository childrenProfileRepository,
            IImageUploadService imageUploadService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _vaccineHistoryRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _childrenProfileRepository = childrenProfileRepository ?? throw new ArgumentNullException(nameof(childrenProfileRepository));
            _imageUploadService = imageUploadService ?? throw new ArgumentNullException(nameof(imageUploadService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<VaccineHistoryResponseDTO>> GetAllVaccineHistoriesAsync()
        {
            var histories = await _vaccineHistoryRepository.GetAllAsync();
            var verifiedHistories = histories.Where(h => h.VerifiedStatus == 1);
            return _mapper.Map<IEnumerable<VaccineHistoryResponseDTO>>(verifiedHistories);
        }

        public async Task<VaccineHistoryResponseDTO?> GetVaccineHistoryByIdAsync(string id)
        {
            var history = await _vaccineHistoryRepository.GetByIdAsync(id);
            if (history == null) return null;
            return _mapper.Map<VaccineHistoryResponseDTO>(history);
        }

        public async Task<VaccineHistoryResponseDTO> AddVaccineHistoryAsync(CreateVaccineHistoryRequestDTO vaccineHistoryDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var profile = await _childrenProfileRepository.GetByIdAsync(vaccineHistoryDto.ProfileId);
                if (profile == null)
                    throw new Exception($"ChildrenProfile with ID {vaccineHistoryDto.ProfileId} not found.");

                var vaccineHistory = _mapper.Map<VaccineHistory>(vaccineHistoryDto);
                vaccineHistory.AccountId = profile.AccountId;
                vaccineHistory.VerifiedStatus = 1;

                await _vaccineHistoryRepository.InsertAsync(vaccineHistory);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<VaccineHistoryResponseDTO>(vaccineHistory);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to add vaccine history: " + ex.Message, ex);
            }
        }

        public async Task<VaccineHistoryResponseDTO?> UpdateVaccineHistoryAsync(string id, CreateVaccineHistoryRequestDTO vaccineHistoryDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingHistory = await _vaccineHistoryRepository.GetByIdAsync(id);
                if (existingHistory == null)
                    return null;

                var profile = await _childrenProfileRepository.GetByIdAsync(vaccineHistoryDto.ProfileId);
                if (profile == null)
                    throw new Exception($"ChildrenProfile with ID {vaccineHistoryDto.ProfileId} not found.");

                _mapper.Map(vaccineHistoryDto, existingHistory);
                existingHistory.AccountId = profile.AccountId;

                await _vaccineHistoryRepository.UpdateAsync(existingHistory);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<VaccineHistoryResponseDTO>(existingHistory);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to update vaccine history: " + ex.Message, ex);
            }
        }

        public async Task<VaccineHistoryResponseDTO> SendVaccineCertificateAsync(SendVaccineCertificateRequestDTO certificateDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var profile = await _childrenProfileRepository.GetByIdAsync(certificateDto.ProfileId);
                if (profile == null)
                    throw new Exception($"ChildrenProfile with ID {certificateDto.ProfileId} not found.");

                // Upload file ảnh nếu có
                string? docUrl = certificateDto.DocumentationProvided != null
                    ? await _imageUploadService.UploadImageAsync(certificateDto.DocumentationProvided): null;

                var vaccineHistory = _mapper.Map<VaccineHistory>(certificateDto);
                vaccineHistory.AccountId = profile.AccountId;
                vaccineHistory.DocumentationProvided = docUrl; // Gán URL sau khi upload
                vaccineHistory.VerifiedStatus = certificateDto.VerifiedStatus; // 0: Chờ xác thực
                vaccineHistory.VaccinedStatus = 0; // Không cần trạng thái tiêm khi gửi chứng nhận
                vaccineHistory.DosedNumber = 0; // Mặc định 0
                vaccineHistory.AdministeredDate = DateTime.Now; // Gán mặc định

                await _vaccineHistoryRepository.InsertAsync(vaccineHistory);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<VaccineHistoryResponseDTO>(vaccineHistory);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to send vaccine certificate: " + ex.Message, ex);
            }
        }

        public async Task<IEnumerable<VaccineHistoryResponseDTO>> GetPendingCertificatesAsync()
        {
            var histories = await _vaccineHistoryRepository.GetAllAsync();
            var pendingHistories = histories.Where(h => h.VerifiedStatus == 0 || h.VerifiedStatus == 2);
            return _mapper.Map<IEnumerable<VaccineHistoryResponseDTO>>(pendingHistories);
        }

        public async Task<VaccineHistoryResponseDTO?> VerifyCertificateAsync(string id, UpdateDocumentVaccineHistoryRequestDTO vaccineHistoryDto, bool isAccepted)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingHistory = await _vaccineHistoryRepository.GetByIdAsync(id);
                if (existingHistory == null)
                    return null;

                if (existingHistory.VerifiedStatus != 0)
                    throw new Exception("This certificate has already been verified.");

                if (isAccepted)
                {
                    _mapper.Map(vaccineHistoryDto, existingHistory);
                    existingHistory.VerifiedStatus = 1; // Accept
                                                        
                }
                else
                {
                    existingHistory.VerifiedStatus = 2; // Deny
                    if (!string.IsNullOrEmpty(vaccineHistoryDto.Notes))
                        existingHistory.Notes = vaccineHistoryDto.Notes; // Cập nhật lý do từ chối nếu có
                }

                await _vaccineHistoryRepository.UpdateAsync(existingHistory);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<VaccineHistoryResponseDTO>(existingHistory);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to verify vaccine certificate: " + ex.Message, ex);
            }
        }

    }
}