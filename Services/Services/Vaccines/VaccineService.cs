using IRepositories.Entity.Vaccines;
using IRepositories.IRepository.Vaccines;
using IRepositories.IRepository;
using IServices.Interfaces.Vaccines;
using ModelViews.Requests.Vaccine;
using ModelViews.Responses.Vaccine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Services.Services.Vaccines
{
    public class VaccineService : IVaccineService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVaccineRepository _repository;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string ImgBBApiKey = "bae68497dea95ef8d4911c8d98f34b5c"; // Thay bằng API key của bạn

        public VaccineService(IUnitOfWork unitOfWork, IVaccineRepository repository, IHttpClientFactory httpClientFactory)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IEnumerable<VaccineResponseDTO>> GetAllVaccinesAsync()
        {
            var vaccines = await _repository.Entities
                .Where(v => v.Status != "0") // Chỉ lấy các vaccine active
                .ToListAsync();

            return vaccines.Select(v => new VaccineResponseDTO
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
                Image = v.Image
            }).ToList();
        }

        public async Task<VaccineResponseDTO?> GetVaccineByIdAsync(string id)
        {
            var vaccine = await _repository.GetByIdAsync(id);
            if (vaccine == null || vaccine.Status == "0") return null;

            return new VaccineResponseDTO
            {
                Id = vaccine.Id,
                Name = vaccine.Name,
                IngredientsDescription = vaccine.IngredientsDescription,
                UnitOfVolume = vaccine.UnitOfVolume,
                MinAge = vaccine.MinAge,
                MaxAge = vaccine.MaxAge,
                BetweenPeriod = vaccine.BetweenPeriod,
                QuantityAvailable = vaccine.QuantityAvailable,
                Price = vaccine.Price,
                ProductionDate = vaccine.ProductionDate,
                ExpirationDate = vaccine.ExpirationDate,
                Status = vaccine.Status,
                VaccineCategoryId = vaccine.VaccineCategoryId,
                BatchId = vaccine.BatchId,
                Image = vaccine.Image
            };
        }

        public async Task<VaccineResponseDTO> AddVaccineAsync(VaccineRequestDTO vaccineDto)
        {
            string imageUrl = vaccineDto.Image != null ? await UploadImageToImgBB(vaccineDto.Image) : null;

            var vaccine = new Vaccine
            {
                Id = Guid.NewGuid().ToString(),
                Name = vaccineDto.Name,
                IngredientsDescription = vaccineDto.IngredientsDescription,
                UnitOfVolume = vaccineDto.UnitOfVolume,
                MinAge = vaccineDto.MinAge,
                MaxAge = vaccineDto.MaxAge,
                BetweenPeriod = vaccineDto.BetweenPeriod,
                QuantityAvailable = vaccineDto.QuantityAvailable,
                Price = vaccineDto.Price,
                ProductionDate = vaccineDto.ProductionDate,
                ExpirationDate = vaccineDto.ExpirationDate,
                Status = "1", // Mặc định active khi tạo mới
                VaccineCategoryId = vaccineDto.VaccineCategoryId,
                BatchId = vaccineDto.BatchId,
                Image = imageUrl
            };

            await _repository.InsertAsync(vaccine);
            await _unitOfWork.SaveAsync();

            return new VaccineResponseDTO
            {
                Id = vaccine.Id,
                Name = vaccine.Name,
                IngredientsDescription = vaccine.IngredientsDescription,
                UnitOfVolume = vaccine.UnitOfVolume,
                MinAge = vaccine.MinAge,
                MaxAge = vaccine.MaxAge,
                BetweenPeriod = vaccine.BetweenPeriod,
                QuantityAvailable = vaccine.QuantityAvailable,
                Price = vaccine.Price,
                ProductionDate = vaccine.ProductionDate,
                ExpirationDate = vaccine.ExpirationDate,
                Status = vaccine.Status,
                VaccineCategoryId = vaccine.VaccineCategoryId,
                BatchId = vaccine.BatchId,
                Image = vaccine.Image
            };
        }

        public async Task UpdateVaccineAsync(string id, VaccineRequestDTO vaccineDto)
        {
            var existingVaccine = await _repository.GetByIdAsync(id);
            if (existingVaccine == null || existingVaccine.Status == "0")
                throw new Exception("Vaccine not found.");

            string imageUrl = vaccineDto.Image != null ? await UploadImageToImgBB(vaccineDto.Image) : existingVaccine.Image;

            existingVaccine.Name = vaccineDto.Name;
            existingVaccine.IngredientsDescription = vaccineDto.IngredientsDescription;
            existingVaccine.UnitOfVolume = vaccineDto.UnitOfVolume;
            existingVaccine.MinAge = vaccineDto.MinAge;
            existingVaccine.MaxAge = vaccineDto.MaxAge;
            existingVaccine.BetweenPeriod = vaccineDto.BetweenPeriod;
            existingVaccine.QuantityAvailable = vaccineDto.QuantityAvailable;
            existingVaccine.Price = vaccineDto.Price;
            existingVaccine.ProductionDate = vaccineDto.ProductionDate;
            existingVaccine.ExpirationDate = vaccineDto.ExpirationDate;
            existingVaccine.VaccineCategoryId = vaccineDto.VaccineCategoryId;
            existingVaccine.BatchId = vaccineDto.BatchId;
            existingVaccine.Image = imageUrl;

            await _repository.UpdateAsync(existingVaccine);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteVaccineAsync(string id)
        {
            var vaccine = await _repository.GetByIdAsync(id);
            if (vaccine == null || vaccine.Status == "0")
                throw new Exception("Vaccine not found.");

            vaccine.Status = "0"; // Soft delete

            await _repository.UpdateAsync(vaccine);
            await _unitOfWork.SaveAsync();
        }

        private async Task<string> UploadImageToImgBB(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new Exception("Invalid image file.");

            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();
            string base64Image = Convert.ToBase64String(imageBytes);

            using var client = _httpClientFactory.CreateClient();
            using var formData = new MultipartFormDataContent
            {
                { new StringContent(ImgBBApiKey), "key" },
                { new StringContent(base64Image), "image" }
            };

            var response = await client.PostAsync("https://api.imgbb.com/1/upload", formData);
            var responseBody = await response.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;

            if (!root.GetProperty("success").GetBoolean())
                throw new Exception("Image upload failed.");

            return root.GetProperty("data").GetProperty("url").GetString();
        }
    }
}