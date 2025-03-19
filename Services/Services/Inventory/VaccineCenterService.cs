using AutoMapper;
using IRepositories.Entity.Inventory;
using IRepositories.IRepository;
using IRepositories.IRepository.Inventory;
using IServices.Interfaces.Inventory;
using ModelViews.Requests.VaccineCenter;
using ModelViews.Responses.VaccineCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services.Inventory
{
    public class VaccineCenterService : IVaccineCenterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVaccineCenterRepository _vaccineCenterRepository;
        private readonly IMapper _mapper;

        public VaccineCenterService(
            IUnitOfWork unitOfWork,
            IVaccineCenterRepository vaccineCenterRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _vaccineCenterRepository = vaccineCenterRepository ?? throw new ArgumentNullException(nameof(vaccineCenterRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IList<VaccineCenterResponseDTO>> GetAllAsync()
        {
            var centers = await _vaccineCenterRepository.GetAllAsync();
            return _mapper.Map<IList<VaccineCenterResponseDTO>>(
                centers.Where(c => c.Status == "1"));
        }

        public async Task<VaccineCenterResponseDTO?> GetByIdAsync(string id)
        {
            var center = await _vaccineCenterRepository.GetByIdAsync(id);
            if (center == null || center.Status != "1")
                return null;
            return _mapper.Map<VaccineCenterResponseDTO>(center);
        }

        public async Task<VaccineCenterResponseDTO> AddAsync(VaccineCenterRequestDTO centerDto)
        {
            var existingCenter = (await _vaccineCenterRepository.GetAllAsync())
                .FirstOrDefault(c => c.Name == centerDto.Name && c.Status == "1");

            if (existingCenter != null)
                throw new Exception("A vaccine center with this name already exists.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var newCenter = _mapper.Map<VaccineCenter>(centerDto);
                newCenter.Status = "1"; // Gán thủ công Status

                await _vaccineCenterRepository.InsertAsync(newCenter);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<VaccineCenterResponseDTO>(newCenter);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to create vaccine center: " + ex.Message, ex);
            }
        }

        public async Task UpdateAsync(VaccineCenterUpdateDTO centerDto)
        {
            var existingCenter = await _vaccineCenterRepository.GetByIdAsync(centerDto.Id);
            if (existingCenter == null || existingCenter.Status != "1")
                throw new Exception("Vaccine center not found or is inactive.");

            var duplicateCenter = (await _vaccineCenterRepository.GetAllAsync())
                .FirstOrDefault(c => c.Name == centerDto.Name && c.Id != centerDto.Id && c.Status == "1");
            if (duplicateCenter != null)
                throw new Exception("Another vaccine center with this name already exists.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _mapper.Map(centerDto, existingCenter);

                await _vaccineCenterRepository.UpdateAsync(existingCenter);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to update vaccine center: " + ex.Message, ex);
            }
        }

        public async Task DeleteAsync(VaccineCenterDeleteDTO centerDto)
        {
            var existingCenter = await _vaccineCenterRepository.GetByIdAsync(centerDto.Id);
            if (existingCenter == null || existingCenter.Status != "1")
                throw new Exception("Vaccine center not found or is inactive.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                existingCenter.Status = "0"; // Soft delete chỉ dùng Status

                await _vaccineCenterRepository.UpdateAsync(existingCenter);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to delete vaccine center: " + ex.Message, ex);
            }
        }

        public async Task<IList<VaccineCenterResponseDTO>> GetByNameAsync(string name)
        {
            var centers = await _vaccineCenterRepository.GetAllAsync();
            return _mapper.Map<IList<VaccineCenterResponseDTO>>(
                centers.Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase) && c.Status == "1"));
        }
    }
}