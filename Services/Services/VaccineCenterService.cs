using ModelViews.Requests.VaccineCenter;
using ModelViews.Responses.VaccineCenter;
using IRepositories.Entity;
using IRepositories.IRepository;
using IServices.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using IRepositories.Entity.Inventory;

namespace Services.Services
{
    public class VaccineCenterService : IVaccineCenterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<VaccineCenter> _vaccineCenterRepository;

        public VaccineCenterService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _vaccineCenterRepository = _unitOfWork.GetRepository<VaccineCenter>();
        }

        public async Task<VaccineCenterResponseDTO> AddAsync(VaccineCenterRequestDTO centerDto)
        {
            var existingCenter = (await _vaccineCenterRepository.GetAllAsync())
                .FirstOrDefault(vc => vc.Name == centerDto.Name);

            if (existingCenter != null)
            {
                throw new Exception("A vaccine center with this name already exists.");
            }

            var newCenter = new VaccineCenter
            {
                Id = Guid.NewGuid().ToString(),
                Name = centerDto.Name,
                Location = centerDto.Location,
                ContactNumber = centerDto.ContactNumber,
                Email = centerDto.Email,
                Status = centerDto.Status
            };

            // Begin transaction
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _vaccineCenterRepository.InsertAsync(newCenter);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new VaccineCenterResponseDTO
                {
                    Id = newCenter.Id,
                    Name = newCenter.Name,
                    Location = newCenter.Location,
                    ContactNumber = newCenter.ContactNumber,
                    Email = newCenter.Email,
                    Status = newCenter.Status
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdateAsync(VaccineCenterUpdateDTO centerDto)
        {
            var existingCenter = await _vaccineCenterRepository.GetByIdAsync(centerDto.Id);
            if (existingCenter == null)
            {
                throw new Exception("Vaccine center not found.");
            }

            // Begin transaction
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                existingCenter.Name = centerDto.Name;
                existingCenter.Location = centerDto.Location;
                existingCenter.ContactNumber = centerDto.ContactNumber;
                existingCenter.Email = centerDto.Email;
                existingCenter.Status = centerDto.Status;

                await _vaccineCenterRepository.UpdateAsync(existingCenter);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<VaccineCenterResponseDTO?> GetByIdAsync(string id)
        {
            var center = await _vaccineCenterRepository.GetByIdAsync(id);
            if (center == null) return null;

            return new VaccineCenterResponseDTO
            {
                Id = center.Id,
                Name = center.Name,
                Location = center.Location,
                ContactNumber = center.ContactNumber,
                Email = center.Email,
                Status = center.Status
            };
        }

        public async Task DeleteAsync(VaccineCenterDeleteDTO centerDto)
        {
            var existingCenter = await _vaccineCenterRepository.GetByIdAsync(centerDto.Id);
            if (existingCenter == null)
            {
                throw new Exception("Vaccine center not found.");
            }

            // Begin transaction
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _vaccineCenterRepository.DeleteAsync(centerDto.Id);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<IList<VaccineCenterResponseDTO>> GetAllAsync()
        {
            var centers = await _vaccineCenterRepository.GetAllAsync();

            // Map VaccineCenter entities to VaccineCenterResponseDTO
            var centerDtos = centers.Select(center => new VaccineCenterResponseDTO
            {
                Id = center.Id,
                Name = center.Name,
                Location = center.Location,
                ContactNumber = center.ContactNumber,
                Email = center.Email,
                Status = center.Status
            }).ToList();

            return centerDtos;
        }

        public async Task<IList<VaccineCenterResponseDTO>> GetByNameAsync(string name)
        {
            var centers = (await _vaccineCenterRepository.GetAllAsync())
                          .Where(vc => vc.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                          .ToList();

            if (centers.Count == 0)
            {
                return new List<VaccineCenterResponseDTO>();
            }

            return centers.Select(center => new VaccineCenterResponseDTO
            {
                Id = center.Id,
                Name = center.Name,
                Location = center.Location,
                ContactNumber = center.ContactNumber,
                Email = center.Email,
                Status = center.Status
            }).ToList();
        }

    }
}
