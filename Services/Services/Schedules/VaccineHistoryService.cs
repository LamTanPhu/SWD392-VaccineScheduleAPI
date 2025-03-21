using AutoMapper;
using IRepositories.Entity.Schedules;
using IRepositories.Entity.Vaccines;
using IRepositories.IRepository;
using IRepositories.IRepository.Schedules;
using IRepositories.IRepository.Vaccines;
using IServices.Interfaces.Schedules;
using ModelViews.Requests.History;
using ModelViews.Responses.VaccineHistory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Schedules
{
    public class VaccineHistoryService : IVaccineHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVaccineHistoryRepository _repository;
        private readonly IMapper _mapper;

        public VaccineHistoryService(
            IUnitOfWork unitOfWork,
            IVaccineHistoryRepository repository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<VaccineHistoryResponseDTO>> GetAllVaccineHistoriesAsync()
        {
            var histories = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<VaccineHistoryResponseDTO>>(histories);
        }

        public async Task<VaccineHistoryResponseDTO?> GetVaccineHistoryByIdAsync(string id)
        {
            var history = await _repository.GetByIdAsync(id);
            if (history == null) return null;
            return _mapper.Map<VaccineHistoryResponseDTO>(history);
        }

        public async Task<VaccineHistoryResponseDTO> AddVaccineHistoryAsync(CreateVaccineHistoryRequestDTO vaccineHistoryDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var vaccineHistory = _mapper.Map<VaccineHistory>(vaccineHistoryDto);

                await _repository.InsertAsync(vaccineHistory);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<VaccineHistoryResponseDTO>(vaccineHistory);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to add vaccine history.", ex);
            }
        }

        public async Task<VaccineHistoryResponseDTO?> UpdateVaccineHistoryAsync(string id, CreateVaccineHistoryRequestDTO vaccineHistoryDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingHistory = await _repository.GetByIdAsync(id);
                if (existingHistory == null)
                    return null;

                _mapper.Map(vaccineHistoryDto, existingHistory);

                await _repository.UpdateAsync(existingHistory);
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