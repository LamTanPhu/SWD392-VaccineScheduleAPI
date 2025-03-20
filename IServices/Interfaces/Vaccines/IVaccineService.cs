using ModelViews.Requests.Vaccine;
using ModelViews.Responses.Vaccine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IServices.Interfaces.Vaccines
{
    public interface IVaccineService
    {
        Task<IEnumerable<VaccineResponseDTO>> GetAllVaccinesAsync();
        Task<VaccineResponseDTO?> GetVaccineByIdAsync(string id);
        Task<VaccineResponseDTO> AddVaccineAsync(VaccineRequestDTO vaccine);
        Task<VaccineResponseDTO> UpdateVaccineAsync(string id, VaccineRequestDTO vaccineDto);
        Task DeleteVaccineAsync(string id); 
    }


}
