using IRepositories.Entity.Vaccines;
using IRepositories.IRepository.Vaccines;
using IRepositories.IRepository;
using IServices.Interfaces.Vaccines;
using ModelViews.Requests.Vaccine;
using ModelViews.Responses.Vaccine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Vaccines
{
    public class VaccineService : IVaccineService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVaccineRepository _repository;

        public VaccineService(IUnitOfWork unitOfWork, IVaccineRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<IEnumerable<VaccineResponseDTO>> GetAllVaccinesAsync()
        {
            var vaccines = await _repository.GetAllAsync();
            return vaccines.Select(v => new VaccineResponseDTO
            {
                Id = v.Id,
                Name = v.Name,
                QuantityAvailable = v.QuantityAvailable,
                Price = v.Price,
                Status = v.Status,
                VaccineCategoryId = v.VaccineCategoryId,
                BatchId = v.BatchId
            }).ToList();
        }

        public async Task<VaccineResponseDTO?> GetVaccineByIdAsync(string id)
        {
            var vaccine = await _repository.GetByIdAsync(id);
            if (vaccine == null) return null;
            return new VaccineResponseDTO
            {
                Id = vaccine.Id,
                Name = vaccine.Name,
                QuantityAvailable = vaccine.QuantityAvailable,
                Price = vaccine.Price,
                Status = vaccine.Status,
                VaccineCategoryId = vaccine.VaccineCategoryId,
                BatchId = vaccine.BatchId
            };
        }

        public async Task AddVaccineAsync(VaccineRequestDTO vaccineDto)
        {
            var vaccine = new Vaccine
            {
                Name = vaccineDto.Name,
                QuantityAvailable = vaccineDto.QuantityAvailable,
                Price = vaccineDto.Price,
                Status = vaccineDto.Status,
                VaccineCategoryId = vaccineDto.VaccineCategoryId,
                BatchId = vaccineDto.BatchId
            };
            await _repository.InsertAsync(vaccine);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateVaccineAsync(string id, VaccineRequestDTO vaccineDto)
        {
            var existingVaccine = await _repository.GetByIdAsync(id);
            if (existingVaccine == null)
                throw new Exception("Vaccine not found.");

            existingVaccine.Name = vaccineDto.Name;
            existingVaccine.QuantityAvailable = vaccineDto.QuantityAvailable;
            existingVaccine.Price = vaccineDto.Price;
            existingVaccine.Status = vaccineDto.Status;
            existingVaccine.VaccineCategoryId = vaccineDto.VaccineCategoryId;
            existingVaccine.BatchId = vaccineDto.BatchId;
            await _repository.UpdateAsync(existingVaccine);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteVaccineAsync(string id)
        {
            await _repository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }
    }
}
