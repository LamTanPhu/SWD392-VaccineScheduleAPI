using ModelViews.Requests.Manufacturer;
using ModelViews.Responses.Manufacturer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace IServices.Interfaces.Inventory
{
    public interface IManufacturerService
    {
        Task<IList<ManufacturerResponseDto>> GetAllManufacturersAsync();
        Task<ManufacturerResponseDto?> GetManufacturerByIdAsync(string id);
        Task<ManufacturerResponseDto?> GetManufacturerByNameAsync(string name);
        Task<ManufacturerResponseDto> AddManufacturerAsync(ManufacturerRequestDto manufacturerDto);
        Task UpdateManufacturerAsync(string id, ManufacturerRequestDto manufacturerDto);
        Task DeleteManufacturerAsync(string id);
    }
}


