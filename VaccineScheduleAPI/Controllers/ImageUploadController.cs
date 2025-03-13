using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        private const string ImgBBApiKey = "bae68497dea95ef8d4911c8d98f34b5c"; // Replace with your API key
        private const string ImgBBUploadUrl = "https://api.imgbb.com/1/upload";

        private readonly HttpClient _httpClient;

        public ImageUploadController(IHttpClientFactory httpClientFactory) // ✅ Correct Fix
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image uploaded.");
            }

            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();
            string base64Image = Convert.ToBase64String(imageBytes);

            using var formData = new MultipartFormDataContent
            {
                { new StringContent(ImgBBApiKey), "key" },
                { new StringContent(base64Image), "image" }
            };

            var response = await _httpClient.PostAsync(ImgBBUploadUrl, formData);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, responseBody);
            }

            return Ok(responseBody);
        }
    }
}
