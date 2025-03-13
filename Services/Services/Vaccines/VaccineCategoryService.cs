using IRepositories.Entity.Vaccines;
using IRepositories.IRepository;
using IRepositories.IRepository.Vaccines;
using IServices.Interfaces.Vaccines;
using ModelViews.Requests.VaccineCategory;
using ModelViews.Responses.VaccineCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Services.Services.Vaccines
{
    public class VaccineCategoryService : IVaccineCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVaccineCategoryRepository _categoryRepository;
        private readonly IVaccineRepository _vaccineRepository;

        public VaccineCategoryService(IUnitOfWork unitOfWork,
                                     IVaccineCategoryRepository categoryRepository,
                                     IVaccineRepository vaccineRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _vaccineRepository = vaccineRepository ?? throw new ArgumentNullException(nameof(vaccineRepository));
        }

        public async Task<IEnumerable<VaccineCategoryResponseDTO>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.Entities
                .Include(c => c.Vaccines) // Eager load Vaccines
                .Where(c => c.DeletedTime == null)
                .ToListAsync();

            return categories.Select(c => new VaccineCategoryResponseDTO
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
                Description = c.Description,
                Status = c.Status,
                ParentCategoryId = c.ParentCategoryId,
                Vaccines = c.Vaccines
                    .Where(v => v.DeletedTime == null)
                    .Select(v => new ModelViews.Responses.Vaccine.VaccineResponseDTO
                    {
                        Id = v.Id,
                        Name = v.Name,
                        IngredientsDescription = v.IngredientsDescription,
                        UnitOfVolume = v.UnitOfVolume,
                        MinAge = v.MinAge,
                        MaxAge = v.MaxAge,
                        BetweenPeriod = v.BetweenPeriod,
                        QuantityAvailable = v.QuantityAvailable,
                        Price = v.Price,
                        ProductionDate = v.ProductionDate,
                        ExpirationDate = v.ExpirationDate,
                        Status = v.Status,
                        VaccineCategoryId = v.VaccineCategoryId,
                        BatchId = v.BatchId,
                        Image = v.Image,
                        ManufacturerName = v.Batch?.Manufacturer?.Name,
                        ManufacturerCountry = v.Batch?.Manufacturer?.CountryName
                    }).ToList()
            }).ToList();
        }

        public async Task<VaccineCategoryResponseDTO?> GetCategoryByIdAsync(string id)
        {
            var category = await _categoryRepository.Entities
                .Include(c => c.Vaccines)
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (category == null) return null;

            return new VaccineCategoryResponseDTO
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                Description = category.Description,
                Status = category.Status,
                ParentCategoryId = category.ParentCategoryId,
                Vaccines = category.Vaccines
                    .Where(v => v.DeletedTime == null)
                    .Select(v => new ModelViews.Responses.Vaccine.VaccineResponseDTO
                    {
                        Id = v.Id,
                        Name = v.Name,
                        IngredientsDescription = v.IngredientsDescription,
                        UnitOfVolume = v.UnitOfVolume,
                        MinAge = v.MinAge,
                        MaxAge = v.MaxAge,
                        BetweenPeriod = v.BetweenPeriod,
                        QuantityAvailable = v.QuantityAvailable,
                        Price = v.Price,
                        ProductionDate = v.ProductionDate,
                        ExpirationDate = v.ExpirationDate,
                        Status = v.Status,
                        VaccineCategoryId = v.VaccineCategoryId,
                        BatchId = v.BatchId,
                        Image = v.Image,
                        ManufacturerName = v.Batch?.Manufacturer?.Name,
                        ManufacturerCountry = v.Batch?.Manufacturer?.CountryName
                    }).ToList()
            };
        }

        public async Task AddCategoryAsync(VaccineCategoryRequestDTO categoryDto)
        {
            var category = new VaccineCategory
            {
                CategoryName = categoryDto.CategoryName,
                Description = categoryDto.Description,
                Status = categoryDto.Status,
                ParentCategoryId = categoryDto.ParentCategoryId
            };
            await _categoryRepository.InsertAsync(category);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateCategoryAsync(string id, VaccineCategoryRequestDTO categoryDto)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
                throw new Exception("Vaccine category not found.");

            existingCategory.CategoryName = categoryDto.CategoryName;
            existingCategory.Description = categoryDto.Description;
            existingCategory.Status = categoryDto.Status;
            existingCategory.ParentCategoryId = categoryDto.ParentCategoryId;
            await _categoryRepository.UpdateAsync(existingCategory);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteCategoryAsync(string id)
        {
            await _categoryRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }
    }
}