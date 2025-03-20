using AutoMapper;
using IRepositories.Entity.Inventory;
using IRepositories.IRepository;
using IRepositories.IRepository.Inventory;
using IServices.Interfaces.Inventory;
using ModelViews.Requests.Manufacturer;
using ModelViews.Responses.Manufacturer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services.Inventory
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IMapper _mapper;

        public ManufacturerService(
            IUnitOfWork unitOfWork,
            IManufacturerRepository manufacturerRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _manufacturerRepository = manufacturerRepository ?? throw new ArgumentNullException(nameof(manufacturerRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IList<ManufacturerResponseDto>> GetAllManufacturersAsync()
        {
            var manufacturers = await _manufacturerRepository.GetAllAsync();
            return _mapper.Map<IList<ManufacturerResponseDto>>(
                manufacturers.Where(m => m.ActiveStatus == true));
        }

        public async Task<ManufacturerResponseDto?> GetManufacturerByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Manufacturer ID is required.");

            var manufacturer = await _manufacturerRepository.GetByIdAsync(id);
            if (manufacturer == null || manufacturer.ActiveStatus != true)
                return null;

            return _mapper.Map<ManufacturerResponseDto>(manufacturer);
        }

        public async Task<ManufacturerResponseDto?> GetManufacturerByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Manufacturer name is required.");

            var manufacturer = await _manufacturerRepository.GetByNameAsync(name);
            if (manufacturer == null || manufacturer.ActiveStatus != true)
                return null;

            return _mapper.Map<ManufacturerResponseDto>(manufacturer);
        }

        public async Task<ManufacturerResponseDto> AddManufacturerAsync(ManufacturerRequestDto manufacturerDto)
        {
            if (manufacturerDto == null)
                throw new ArgumentNullException(nameof(manufacturerDto), "Manufacturer data is required.");

            var existingManufacturer = (await _manufacturerRepository.GetAllAsync())
                .FirstOrDefault(m => m.Name == manufacturerDto.Name && m.ActiveStatus == true);
            if (existingManufacturer != null)
                throw new InvalidOperationException("A manufacturer with this name already exists.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var manufacturer = _mapper.Map<Manufacturer>(manufacturerDto);
                manufacturer.ActiveStatus = true; // Gán thủ công ActiveStatus

                await _manufacturerRepository.InsertAsync(manufacturer);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<ManufacturerResponseDto>(manufacturer);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to create manufacturer: " + ex.Message, ex);
            }
        }

        public async Task UpdateManufacturerAsync(string id, ManufacturerRequestDto manufacturerDto)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Manufacturer ID is required.");
            if (manufacturerDto == null)
                throw new ArgumentNullException(nameof(manufacturerDto), "Manufacturer data is required.");

            var existingManufacturer = await _manufacturerRepository.GetByIdAsync(id);
            if (existingManufacturer == null || existingManufacturer.ActiveStatus != true)
                throw new InvalidOperationException("Manufacturer not found or is inactive.");

            var duplicateManufacturer = (await _manufacturerRepository.GetAllAsync())
                .FirstOrDefault(m => m.Name == manufacturerDto.Name && m.Id != id && m.ActiveStatus == true);
            if (duplicateManufacturer != null)
                throw new InvalidOperationException("Another manufacturer with this name already exists.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _mapper.Map(manufacturerDto, existingManufacturer);

                await _manufacturerRepository.UpdateAsync(existingManufacturer);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to update manufacturer: " + ex.Message, ex);
            }
        }

        public async Task DeleteManufacturerAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Manufacturer ID is required.");

            var existingManufacturer = await _manufacturerRepository.GetByIdAsync(id);
            if (existingManufacturer == null || existingManufacturer.ActiveStatus != true)
                throw new InvalidOperationException("Manufacturer not found or is inactive.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                existingManufacturer.ActiveStatus = false; // Soft delete với ActiveStatus

                await _manufacturerRepository.UpdateAsync(existingManufacturer);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to delete manufacturer: " + ex.Message, ex);
            }
        }
    }
}