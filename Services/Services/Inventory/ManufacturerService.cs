using IRepositories.Entity;
using IRepositories.IRepository;
using IRepositories.IRepository.Inventory;
using IServices.Interfaces.Inventory;
using ModelViews.Requests.Manufacturer;
using ModelViews.Responses.Manufacturer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Inventory
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IManufacturerRepository _manufacturerRepository;

        public ManufacturerService(IUnitOfWork unitOfWork, IManufacturerRepository manufacturerRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _manufacturerRepository = manufacturerRepository ?? throw new ArgumentNullException(nameof(manufacturerRepository));
        }

        public async Task<IEnumerable<ManufacturerResponseDto>> GetAllManufacturersAsync()
        {
            var manufacturers = await _manufacturerRepository.GetAllAsync();
            return manufacturers.Select(m => new ManufacturerResponseDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                CountryName = m.CountryName,
                CountryCode = m.CountryCode,
                ActiveStatus = m.ActiveStatus
            }).ToList();
        }

        public async Task<ManufacturerResponseDto?> GetManufacturerByIdAsync(string id)
        {
            var manufacturer = await _manufacturerRepository.GetByIdAsync(id);
            if (manufacturer == null) return null;
            return new ManufacturerResponseDto
            {
                Id = manufacturer.Id,
                Name = manufacturer.Name,
                Description = manufacturer.Description,
                CountryName = manufacturer.CountryName,
                CountryCode = manufacturer.CountryCode,
                ActiveStatus = manufacturer.ActiveStatus
            };
        }

        public async Task<ManufacturerResponseDto?> GetManufacturerByNameAsync(string name)
        {
            var manufacturer = await _manufacturerRepository.GetByNameAsync(name);
            if (manufacturer == null) return null;
            return new ManufacturerResponseDto
            {
                Id = manufacturer.Id,
                Name = manufacturer.Name,
                Description = manufacturer.Description,
                CountryName = manufacturer.CountryName,
                CountryCode = manufacturer.CountryCode,
                ActiveStatus = manufacturer.ActiveStatus
            };
        }

        public async Task AddManufacturerAsync(ManufacturerRequestDto manufacturerDto)
        {
            var manufacturer = new Manufacturer
            {
                Name = manufacturerDto.Name,
                Description = manufacturerDto.Description,
                CountryName = manufacturerDto.CountryName,
                CountryCode = manufacturerDto.CountryCode,
                ActiveStatus = manufacturerDto.ActiveStatus
            };

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _manufacturerRepository.InsertAsync(manufacturer);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdateManufacturerAsync(string id, ManufacturerRequestDto manufacturerDto)
        {
            var existingManufacturer = await _manufacturerRepository.GetByIdAsync(id);
            if (existingManufacturer == null)
            {
                throw new Exception("Manufacturer not found.");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                existingManufacturer.Name = manufacturerDto.Name;
                existingManufacturer.Description = manufacturerDto.Description;
                existingManufacturer.CountryName = manufacturerDto.CountryName;
                existingManufacturer.CountryCode = manufacturerDto.CountryCode;
                existingManufacturer.ActiveStatus = manufacturerDto.ActiveStatus;

                await _manufacturerRepository.UpdateAsync(existingManufacturer);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeleteManufacturerAsync(string id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _manufacturerRepository.DeleteAsync(id);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
