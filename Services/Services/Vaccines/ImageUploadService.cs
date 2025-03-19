using IServices.Interfaces.Vaccines;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Services.Vaccines
{
    public class ImageUploadService : IImageUploadService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string ImgBBApiKey = "bae68497dea95ef8d4911c8d98f34b5c";

        public ImageUploadService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> UploadImageAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("Invalid image file.");

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
