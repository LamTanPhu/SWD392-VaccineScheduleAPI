using IRepositories.Entity;
using IRepositories.IRepository;
using IServices.Interfaces;
using ModelViews.Requests.VaccineBatch;
using ModelViews.Responses.VaccineBatch;
using Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class VaccineBatchService : IVaccineBatchService
    {
        private readonly IVaccineBatchRepository _vaccineBatchRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VaccineBatchService(IUnitOfWork unitOfWork, IVaccineBatchRepository vaccineBatchRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            _vaccineBatchRepository = vaccineBatchRepository;
        }

        public async Task<VaccineBatch?> GetByBatchNumberAsync(string batchNumber)
        {
            return await _vaccineBatchRepository.GetByBatchNumberAsync(batchNumber);
        }

        public async Task<AddVaccineBatchResponseDTO> AddBatchAsync(AddVaccineBatchRequestDTO request)
        {
            var newBatch = new VaccineBatch
            {
                BatchNumber = request.BatchNumber,
                Quantity = request.Quantity,
                ManufacturerId = request.ManufacturerId,
                VaccineCenterId = request.VaccineCenterId,
                ActiveStatus = request.ActiveStatus
            };

            var success = await _vaccineBatchRepository.AddBatchAsync(newBatch);
            if (success)
            {
                return new AddVaccineBatchResponseDTO
                {
                    Success = true,
                    Message = "Vaccine batch added successfully.",
                    BatchNumber = newBatch.BatchNumber,
                    Quantity = newBatch.Quantity,
                    ManufacturerId = newBatch.ManufacturerId,
                    VaccineCenterId = newBatch.VaccineCenterId,
                    ActiveStatus = newBatch.ActiveStatus
                };
            }

            return new AddVaccineBatchResponseDTO
            {
                Success = false,
                Message = "Failed to add vaccine batch."
            };
        }

        public async Task<IEnumerable<VaccineBatchResponseDTO>> SearchByNameAsync(string name)
        {
            var batches = await _vaccineBatchRepository.GetAllAsync();
            return batches.Where(vb => vb.BatchNumber.Contains(name)).Select(vb => new VaccineBatchResponseDTO
            {
                BatchNumber = vb.BatchNumber,
                Quantity = vb.Quantity,
                ManufacturerId = vb.ManufacturerId,
                VaccineCenterId = vb.VaccineCenterId,
                ActiveStatus = vb.ActiveStatus
            }).ToList();
        }
    }
}
