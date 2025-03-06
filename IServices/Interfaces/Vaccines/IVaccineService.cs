using ModelViews.Requests.Vaccine;
using ModelViews.Responses.Vaccine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Vaccines
{
    public interface IVaccineService
    {
        Task<IEnumerable<VaccineResponseDTO>> GetAllVaccinesAsync();
        Task<VaccineResponseDTO?> GetVaccineByIdAsync(string id);
        Task AddVaccineAsync(VaccineRequestDTO vaccine);
        Task UpdateVaccineAsync(string id, VaccineRequestDTO vaccineDto);
        Task DeleteVaccineAsync(string id);
    }
}
