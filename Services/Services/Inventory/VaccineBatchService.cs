using AutoMapper;
using IRepositories.Entity.Inventory;
using IRepositories.IRepository;
using IRepositories.IRepository.Inventory;
using IServices.Interfaces.Inventory;
using ModelViews.Requests.VaccineBatch;
using ModelViews.Responses.VaccineBatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services.Inventory
{
    public class VaccineBatchService : IVaccineBatchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVaccineBatchRepository _vaccineBatchRepository;
        private readonly IMapper _mapper;

        public VaccineBatchService(
            IUnitOfWork unitOfWork,
            IVaccineBatchRepository vaccineBatchRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _vaccineBatchRepository = vaccineBatchRepository ?? throw new ArgumentNullException(nameof(vaccineBatchRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<VaccineBatchResponseDTO>> GetAllAsync()
        {
            var batches = await _vaccineBatchRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<VaccineBatchResponseDTO>>(batches.Where(b => b.ActiveStatus != "0"));
        }

        public async Task<VaccineBatchResponseDTO?> GetByBatchNumberAsync(string batchNumber)
        {
            var batch = await _vaccineBatchRepository.GetByBatchNumberAsync(batchNumber);
            if (batch == null || batch.ActiveStatus == "0") return null;
            return _mapper.Map<VaccineBatchResponseDTO>(batch);
        }

        public async Task<IEnumerable<VaccineBatchResponseDTO>> SearchByNameAsync(string name)
        {
            var batches = await _vaccineBatchRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<VaccineBatchResponseDTO>>(
                batches.Where(b => b.BatchNumber.Contains(name, StringComparison.OrdinalIgnoreCase) && b.ActiveStatus != "0"));
        }

        public async Task<VaccineBatchResponseDTO> CreateAsync(AddVaccineBatchRequestDTO request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var batch = _mapper.Map<VaccineBatch>(request);
                // Status mặc định là "1" từ BaseEntity, không cần gán lại

                await _vaccineBatchRepository.InsertAsync(batch);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<VaccineBatchResponseDTO>(batch);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to create vaccine batch: " + ex.Message, ex);
            }
        }
    }
}