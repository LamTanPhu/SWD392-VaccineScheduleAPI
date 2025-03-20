using AutoMapper;
using IRepositories.Entity.Vaccines;
using IRepositories.IRepository;
using IRepositories.IRepository.Vaccines;
using IServices.Interfaces.Vaccines;
using Microsoft.EntityFrameworkCore;
using ModelViews.Requests.Vaccine;
using ModelViews.Responses.Vaccine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace Services.Services.Vaccines
{
    public class VaccineService : IVaccineService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVaccineRepository _repository;
        private readonly IImageUploadService _imageUploadService;
        private readonly IMapper _mapper;

        public VaccineService(IUnitOfWork unitOfWork, IVaccineRepository repository, IImageUploadService imageUploadService, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _imageUploadService = imageUploadService ?? throw new ArgumentNullException(nameof(imageUploadService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<VaccineResponseDTO>> GetAllVaccinesAsync()
        {
            var vaccines = await _repository.Entities
                .Where(v => v.Status != "0") // Lọc soft delete
                .Include(v => v.Batch)
                .ThenInclude(b => b.Manufacturer)
                .ToListAsync();

            return _mapper.Map<IEnumerable<VaccineResponseDTO>>(vaccines);
        }

        public async Task<VaccineResponseDTO?> GetVaccineByIdAsync(string id)
        {
            var vaccine = await _repository.GetByIdAsync(id);
            if (vaccine == null || vaccine.Status == "0") return null;

            // Include Batch và Manufacturer nếu cần
            await _repository.Entities
                .Where(v => v.Id == id)
                .Include(v => v.Batch)
                .ThenInclude(b => b.Manufacturer)
                .FirstOrDefaultAsync();

            return _mapper.Map<VaccineResponseDTO>(vaccine);
        }

        public async Task<VaccineResponseDTO> AddVaccineAsync(VaccineRequestDTO vaccineDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                string imageUrl = vaccineDto.Image != null ? await _imageUploadService.UploadImageAsync(vaccineDto.Image) : null;

                var vaccine = _mapper.Map<Vaccine>(vaccineDto);
                vaccine.Status = "1"; 
                vaccine.Image = imageUrl;

                await _repository.InsertAsync(vaccine);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<VaccineResponseDTO>(vaccine);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to add vaccine.", ex);
            }
        }

        public async Task<VaccineResponseDTO?> UpdateVaccineAsync(string id, VaccineRequestDTO vaccineDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingVaccine = await _repository.GetByIdAsync(id);
                if (existingVaccine == null || existingVaccine.Status == "0")
                    return null; 

                string imageUrl = vaccineDto.Image != null ? await _imageUploadService.UploadImageAsync(vaccineDto.Image) : existingVaccine.Image;

                _mapper.Map(vaccineDto, existingVaccine);
                existingVaccine.Image = imageUrl;

                await _repository.UpdateAsync(existingVaccine);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<VaccineResponseDTO>(existingVaccine);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to update vaccine: " + ex.Message, ex);
            }
        }

        public async Task DeleteVaccineAsync(string id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var vaccine = await _repository.GetByIdAsync(id);
                if (vaccine == null || vaccine.Status == "0")
                    throw new Exception("Vaccine not found.");

                vaccine.Status = "0";
                await _repository.UpdateAsync(vaccine);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to delete vaccine.", ex);
            }
        }
    }
}