using ModelViews.Requests.Schedule;
using ModelViews.Responses.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Schedules
{
    public interface IVaccinationScheduleService
    {
        Task<List<ScheduleResponseDTO>> CreateSchedulesAsync(ScheduleRequestDTO request);
        Task<ScheduleResponseDTO> CreateScheduleAsync(CreateScheduleRequestDTO request);
        Task<List<ScheduleResponseDTO>> GetAllSchedulesAsync();
        Task<ScheduleResponseDTO> GetScheduleByIdAsync(string id);
        Task UpdateScheduleAsync(string scheduleID, UpdateScheduleRequestDTO request);
        Task DeleteScheduleAsync(string id);
    }
}
