using IRepositories.Entity.Schedules;
using IRepositories.IRepository.Schedules;
using IRepositories.IRepository;
using IServices.Interfaces.Schedules;
using ModelViews.Requests.Feedback;
using ModelViews.Responses.Feedback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Schedules
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFeedbackRepository _repository;

        public FeedbackService(IUnitOfWork unitOfWork, IFeedbackRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<IEnumerable<FeedbackResponseDTO>> GetAllFeedbacksAsync()
        {
            var feedbacks = await _repository.GetAllAsync();
            return feedbacks.Select(f => new FeedbackResponseDTO
            {
                Id = f.Id,
                OrderId = f.OrderId,
                Rating = f.Rating,
                Comment = f.Comment,
                Status = f.Status
            }).ToList();
        }

        public async Task<FeedbackResponseDTO?> GetFeedbackByIdAsync(string id)
        {
            var feedback = await _repository.GetByIdAsync(id);
            if (feedback == null) return null;
            return new FeedbackResponseDTO
            {
                Id = feedback.Id,
                OrderId = feedback.OrderId,
                Rating = feedback.Rating,
                Comment = feedback.Comment,
                Status = feedback.Status
            };
        }

        public async Task AddFeedbackAsync(FeedbackRequestDTO feedbackDto)
        {
            var feedback = new Feedback
            {
                OrderId = feedbackDto.OrderId,
                Rating = feedbackDto.Rating,
                Comment = feedbackDto.Comment,
                Status = feedbackDto.Status
            };
            await _repository.InsertAsync(feedback);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateFeedbackAsync(string id, FeedbackRequestDTO feedbackDto)
        {
            var existingFeedback = await _repository.GetByIdAsync(id);
            if (existingFeedback == null)
                throw new Exception("Feedback not found.");

            existingFeedback.Rating = feedbackDto.Rating;
            existingFeedback.Comment = feedbackDto.Comment;
            existingFeedback.Status = feedbackDto.Status;
            await _repository.UpdateAsync(existingFeedback);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteFeedbackAsync(string id)
        {
            await _repository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }
    }
}
