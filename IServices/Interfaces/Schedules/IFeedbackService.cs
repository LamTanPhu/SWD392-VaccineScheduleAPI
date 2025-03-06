using ModelViews.Requests.Feedback;
using ModelViews.Responses.Feedback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Schedules
{
    public interface IFeedbackService
    {
        Task<IEnumerable<FeedbackResponseDTO>> GetAllFeedbacksAsync();
        Task<FeedbackResponseDTO?> GetFeedbackByIdAsync(string id);
        Task AddFeedbackAsync(FeedbackRequestDTO feedback);
        Task UpdateFeedbackAsync(string id, FeedbackRequestDTO feedbackDto);
        Task DeleteFeedbackAsync(string id);
    }
}
