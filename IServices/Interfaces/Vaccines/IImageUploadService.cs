using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Vaccines
{
    public interface IImageUploadService
    {
        Task<string> UploadImageAsync(IFormFile image);
    }
}
