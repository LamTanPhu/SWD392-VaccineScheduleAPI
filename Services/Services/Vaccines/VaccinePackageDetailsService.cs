using AutoMapper;
using IRepositories.Entity.Vaccines;
using IRepositories.IRepository;
using IRepositories.IRepository.Vaccines;
using IServices.Interfaces.Vaccines;
using ModelViews.Requests.VaccinePackage;
using ModelViews.Responses.VaccinePackage;
using Repositories.Repository.Vaccines;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Vaccines
{
    public class VaccinePackageDetailsService : IVaccinePackageDetailsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVaccinePackageDetailsRepository _vaccinePackageDetailRepository;
        private readonly IVaccineRepository _vaccineRepository;
        private readonly IVaccinePackageRepository _vaccinePackageRepository;
        private readonly IMapper _mapper;

        public VaccinePackageDetailsService(
        IUnitOfWork unitOfWork,
            IVaccinePackageDetailsRepository vaccinePackageDetailRepository,
            IVaccineRepository vaccineRepository,
            IVaccinePackageRepository vaccinePackageRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _vaccinePackageDetailRepository = vaccinePackageDetailRepository ?? throw new ArgumentNullException(nameof(vaccinePackageDetailRepository));
            _vaccineRepository = vaccineRepository ?? throw new ArgumentNullException(nameof(vaccineRepository));
            _vaccinePackageRepository = vaccinePackageRepository ?? throw new ArgumentNullException(nameof(vaccinePackageRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IList<VaccinePackageDetailsResponseDTO>> GetAllDetailsAsync()
        {
            var details = await _vaccinePackageDetailRepository.GetAllAsync();
            return _mapper.Map<IList<VaccinePackageDetailsResponseDTO>>(details);
        }

        public async Task<VaccinePackageDetailsResponseDTO> GetDetailByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Detail ID is required.");

            var detail = await _vaccinePackageDetailRepository.GetByIdAsync(id);
            if (detail == null)
                throw new InvalidOperationException($"Vaccine package detail with ID {id} not found.");

            return _mapper.Map<VaccinePackageDetailsResponseDTO>(detail);
        }

        public async Task<VaccinePackageDetailsResponseDTO> AddDetailAsync(VaccinePackageDetailsRequestDTO detailDto)
        {
            if (detailDto == null)
                throw new ArgumentNullException(nameof(detailDto), "Detail data is required.");
            if (string.IsNullOrEmpty(detailDto.VaccineId))
                throw new ArgumentException("Vaccine ID is required.");
            if (string.IsNullOrEmpty(detailDto.VaccinePackageId))
                throw new ArgumentException("Vaccine package ID is required.");
            if (detailDto.DoseNumber <= 0)
                throw new ArgumentException("Dose number must be greater than 0.");

            var vaccine = await _vaccineRepository.GetByIdAsync(detailDto.VaccineId);
            if (vaccine == null || vaccine.Status == "0")
                throw new InvalidOperationException($"Vaccine with ID {detailDto.VaccineId} not found or inactive.");

            var package = await _vaccinePackageRepository.GetByIdAsync(detailDto.VaccinePackageId);
            if (package == null || package.PackageStatus != true)
                throw new InvalidOperationException($"Vaccine package with ID {detailDto.VaccinePackageId} not found or inactive.");


            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var detail = _mapper.Map<VaccinePackageDetail>(detailDto);
                await _vaccinePackageDetailRepository.InsertAsync(detail);

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                var createdDetail = await _vaccinePackageDetailRepository.GetByIdAsync(detail.Id);
                return _mapper.Map<VaccinePackageDetailsResponseDTO>(createdDetail);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to add vaccine package detail: " + ex.Message, ex);
            }
        }

        public async Task UpdateDetailAsync(string id, VaccinePackageDetailsRequestDTO detailDto)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Detail ID is required.");
            if (detailDto == null)
                throw new ArgumentNullException(nameof(detailDto), "Detail data is required.");
            if (string.IsNullOrEmpty(detailDto.VaccineId))
                throw new ArgumentException("Vaccine ID is required.");
            if (string.IsNullOrEmpty(detailDto.VaccinePackageId))
                throw new ArgumentException("Vaccine package ID is required.");
            if (detailDto.DoseNumber <= 0)
                throw new ArgumentException("Dose number must be greater than 0.");

            var existingDetail = await _vaccinePackageDetailRepository.GetByIdAsync(id);
            if (existingDetail == null)
                throw new InvalidOperationException($"Vaccine package detail with ID {id} not found.");

            var vaccine = await _vaccineRepository.GetByIdAsync(detailDto.VaccineId);
            if (vaccine == null || vaccine.Status == "0")
                throw new InvalidOperationException($"Vaccine with ID {detailDto.VaccineId} not found or inactive.");

            var package = await _vaccinePackageRepository.GetByIdAsync(detailDto.VaccinePackageId);
            if (package == null || package.PackageStatus != true)
                throw new InvalidOperationException($"Vaccine package with ID {detailDto.VaccinePackageId} not found or inactive.");


            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _mapper.Map(detailDto, existingDetail);
                await _vaccinePackageDetailRepository.UpdateAsync(existingDetail);

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to update vaccine package detail: " + ex.Message, ex);
            }
        }

        public async Task DeleteDetailAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Detail ID is required.");

            var existingDetail = await _vaccinePackageDetailRepository.GetByIdAsync(id);
            if (existingDetail == null)
                throw new InvalidOperationException($"Vaccine package detail with ID {id} not found.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _vaccinePackageDetailRepository.DeleteAsync(id);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to delete vaccine package detail: " + ex.Message, ex);
            }
        }
    }
}