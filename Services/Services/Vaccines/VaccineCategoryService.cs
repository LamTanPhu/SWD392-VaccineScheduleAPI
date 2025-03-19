using AutoMapper;
using IRepositories.Entity.Vaccines;
using IRepositories.IRepository;
using IRepositories.IRepository.Vaccines;
using IServices.Interfaces.Vaccines;
using ModelViews.Requests.VaccineCategory;
using ModelViews.Responses.VaccineCategory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Vaccines
{
    public class VaccineCategoryService : IVaccineCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVaccineCategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public VaccineCategoryService(
            IUnitOfWork unitOfWork,
            IVaccineCategoryRepository categoryRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<VaccineCategoryResponseDTO>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<VaccineCategoryResponseDTO>>(
                categories.Where(c => c.Status == "1"));
        }

        public async Task<VaccineCategoryResponseDTO?> GetByIdAsync(string id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null || category.Status != "1")
                return null;
            return _mapper.Map<VaccineCategoryResponseDTO>(category);
        }

        public async Task<VaccineCategoryResponseDTO> CreateAsync(VaccineCategoryRequestDTO categoryDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (!string.IsNullOrEmpty(categoryDto.ParentCategoryId))
                {
                    var parentCategory = await _categoryRepository.GetByIdAsync(categoryDto.ParentCategoryId);
                    if (parentCategory == null || parentCategory.Status != "1")
                        throw new Exception($"Parent category with ID {categoryDto.ParentCategoryId} does not exist or is inactive.");
                }

                var category = _mapper.Map<VaccineCategory>(categoryDto);
                category.Status = "1"; // Gán thủ công Status

                await _categoryRepository.InsertAsync(category);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<VaccineCategoryResponseDTO>(category);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to create vaccine category: " + ex.Message, ex);
            }
        }

        public async Task UpdateAsync(string id, VaccineCategoryRequestDTO categoryDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingCategory = await _categoryRepository.GetByIdAsync(id);
                if (existingCategory == null || existingCategory.Status != "1")
                    throw new Exception("Vaccine category not found or is inactive.");

                if (!string.IsNullOrEmpty(categoryDto.ParentCategoryId))
                {
                    var parentCategory = await _categoryRepository.GetByIdAsync(categoryDto.ParentCategoryId);
                    if (parentCategory == null || parentCategory.Status != "1")
                        throw new Exception($"Parent category with ID {categoryDto.ParentCategoryId} does not exist or is inactive.");
                    if (categoryDto.ParentCategoryId == id)
                        throw new Exception("A category cannot be its own parent.");
                }

                _mapper.Map(categoryDto, existingCategory);

                await _categoryRepository.UpdateAsync(existingCategory);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to update vaccine category: " + ex.Message, ex);
            }
        }

        public async Task DeleteAsync(string id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null || category.Status != "1")
                    throw new Exception("Vaccine category not found or is inactive.");

                category.Status = "0"; // Soft delete chỉ dùng Status

                await _categoryRepository.UpdateAsync(category);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to delete vaccine category: " + ex.Message, ex);
            }
        }
    }
}