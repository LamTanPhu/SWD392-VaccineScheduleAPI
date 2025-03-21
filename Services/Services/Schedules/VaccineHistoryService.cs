using AutoMapper;
using IRepositories.Entity.Accounts;
using IRepositories.Entity.Schedules;
using IRepositories.Entity.Vaccines;
using IRepositories.IRepository;
using IRepositories.IRepository.Accounts;
using IRepositories.IRepository.Schedules;
using IRepositories.IRepository.Vaccines;
using IServices.Interfaces.Schedules;
using ModelViews.Requests.History;
using ModelViews.Responses.VaccineHistory;
using Repositories.Repository.Accounts;
using Repositories.Repository.Schedules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Schedules
{
    public class VaccineHistoryService : IVaccineHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVaccineHistoryRepository _vaccineHistoryRepository;
        private readonly IChildrenProfileRepository _childrenProfileRepository; 
        private readonly IMapper _mapper;

        public VaccineHistoryService(
            IUnitOfWork unitOfWork,
        IVaccineHistoryRepository repository,
            IChildrenProfileRepository childrenProfileRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _vaccineHistoryRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _childrenProfileRepository = childrenProfileRepository ?? throw new ArgumentNullException(nameof(childrenProfileRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<VaccineHistoryResponseDTO>> GetAllVaccineHistoriesAsync()
        {
            var histories = await _vaccineHistoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<VaccineHistoryResponseDTO>>(histories);
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
                vaccineHistory.AccountId = profile.AccountId; // Gán AccountId từ ChildrenProfile
                vaccineHistory.VerifiedStatus = 1; // Gán mặc định (1: Accept)

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

                _mapper.Map(vaccineHistoryDto, existingHistory);

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


    }
}