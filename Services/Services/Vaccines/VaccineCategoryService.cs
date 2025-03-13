using IRepositories.Entity.Vaccines;
using IRepositories.IRepository.Vaccines;
using IRepositories.IRepository;
using ModelViews.Requests.VaccineCategory;
using ModelViews.Responses.VaccineCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IServices.Interfaces.Vaccines;

namespace Services.Services.Vaccines
{
    public class VaccineCategoryService : IVaccineCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVaccineCategoryRepository _repository;

        public VaccineCategoryService(IUnitOfWork unitOfWork, IVaccineCategoryRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<IEnumerable<VaccineCategoryResponseDTO>> GetAllCategoriesAsync()
        {
            var categories = await _repository.GetAllAsync();
            return categories.Select(c => new VaccineCategoryResponseDTO
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
                Description = c.Description,
                Status = c.Status,
                ParentCategoryId = c.ParentCategoryId
            }).ToList();
        }

        public async Task<VaccineCategoryResponseDTO?> GetCategoryByIdAsync(string id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return null;
            return new VaccineCategoryResponseDTO
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                Description = category.Description,
                Status = category.Status,
                ParentCategoryId = category.ParentCategoryId
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
            await _repository.InsertAsync(category);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateCategoryAsync(string id, VaccineCategoryRequestDTO categoryDto)
        {
            var existingCategory = await _repository.GetByIdAsync(id);
            if (existingCategory == null)
                throw new Exception("Vaccine category not found.");

            existingCategory.CategoryName = categoryDto.CategoryName;
            existingCategory.Description = categoryDto.Description;
            existingCategory.Status = categoryDto.Status;
            existingCategory.ParentCategoryId = categoryDto.ParentCategoryId;
            await _repository.UpdateAsync(existingCategory);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteCategoryAsync(string id)
        {
            await _repository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }
    }
}
