using ModelViews.Requests.Schedule;
using ModelViews.Responses.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Schedules
{
    public interface IVaccineReactionService
    {       
        Task<VaccineReactionResponseDTO> CreateReactionAsync(VaccineReactionRequestDTO request);
        Task<List<VaccineReactionResponseDTO>> GetAllReactionsAsync();
        Task<VaccineReactionResponseDTO> GetReactionByIdAsync(string id);
        Task UpdateReactionAsync(string id, VaccineReactionRequestDTO request);
        Task DeleteReactionAsync(string id);
    }
}
