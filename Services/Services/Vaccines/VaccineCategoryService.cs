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
            // Validate ParentCategoryId if provided
            if (!string.IsNullOrEmpty(categoryDto.ParentCategoryId))
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(categoryDto.ParentCategoryId);
                if (parentCategory == null || parentCategory.DeletedTime != null)
                {
                    throw new Exception($"Parent category with ID {categoryDto.ParentCategoryId} does not exist or has been deleted.");
                }
            }

            var category = new VaccineCategory
            {
                CategoryName = categoryDto.CategoryName,
                Description = categoryDto.Description,
                Status = categoryDto.Status,
                ParentCategoryId = string.IsNullOrEmpty(categoryDto.ParentCategoryId) ? null : categoryDto.ParentCategoryId
            };

            await _categoryRepository.InsertAsync(category);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateCategoryAsync(string id, VaccineCategoryRequestDTO categoryDto)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null || existingCategory.DeletedTime != null)
            {
                throw new Exception("Vaccine category not found or has been deleted.");
            }

            // Validate ParentCategoryId if provided
            if (!string.IsNullOrEmpty(categoryDto.ParentCategoryId))
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(categoryDto.ParentCategoryId);
                if (parentCategory == null || parentCategory.DeletedTime != null)
                {
                    throw new Exception($"Parent category with ID {categoryDto.ParentCategoryId} does not exist or has been deleted.");
                }
                // Prevent self-referencing
                if (categoryDto.ParentCategoryId == id)
                {
                    throw new Exception("A category cannot be its own parent.");
                }
            }

            existingCategory.CategoryName = categoryDto.CategoryName;
            existingCategory.Description = categoryDto.Description;
            existingCategory.Status = categoryDto.Status;
            existingCategory.ParentCategoryId = string.IsNullOrEmpty(categoryDto.ParentCategoryId) ? null : categoryDto.ParentCategoryId;

            await _categoryRepository.UpdateAsync(existingCategory);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteCategoryAsync(string id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null || category.DeletedTime != null)
            {
                throw new Exception("Vaccine category not found or has been deleted.");
            }

            await _categoryRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }
    }
}