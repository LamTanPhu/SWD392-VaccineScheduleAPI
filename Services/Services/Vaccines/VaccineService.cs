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
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Services.Services.Vaccines
{
    public class VaccineService : IVaccineService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVaccineRepository _repository;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string ImgBBApiKey = "bae68497dea95ef8d4911c8d98f34b5c"; // Replace with your actual API key

        public VaccineService(IUnitOfWork unitOfWork, IVaccineRepository repository, IHttpClientFactory httpClientFactory)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _httpClientFactory = httpClientFactory;
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
                BatchId = v.BatchId,
                image = v.Image
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
                BatchId = vaccine.BatchId,
                image = vaccine.Image
            };
        }

        public async Task<VaccineResponseDTO> AddVaccineAsync(VaccineRequestDTO vaccineDto)
        {
            string imageUrl = await UploadImageToImgBB(vaccineDto.image);

            var vaccine = new Vaccine
            {
                Name = vaccineDto.Name,
                QuantityAvailable = vaccineDto.QuantityAvailable,
                Price = vaccineDto.Price,
                Status = vaccineDto.Status,
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
                QuantityAvailable = vaccine.QuantityAvailable,
                Price = vaccine.Price,
                Status = vaccine.Status,
                VaccineCategoryId = vaccine.VaccineCategoryId,
                BatchId = vaccine.BatchId,
                image = vaccine.Image
            };
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
