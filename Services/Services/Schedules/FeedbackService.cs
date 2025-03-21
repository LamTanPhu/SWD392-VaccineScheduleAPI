using IRepositories.Entity.Schedules;
using IRepositories.IRepository.Schedules;
using IRepositories.IRepository;
using IServices.Interfaces.Schedules;
using ModelViews.Requests.Feedback;
using ModelViews.Responses.Feedback;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper; // Thêm namespace AutoMapper

namespace Services.Services.Schedules
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFeedbackRepository _repository;
        private readonly IMapper _mapper; 

        public FeedbackService(IUnitOfWork unitOfWork, IFeedbackRepository repository, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<FeedbackResponseDTO>> GetAllFeedbacksAsync()
        {
            var feedbacks = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<FeedbackResponseDTO>>(feedbacks); 
        }

        public async Task<FeedbackResponseDTO?> GetFeedbackByIdAsync(string id)
        {
            var feedback = await _repository.GetByIdAsync(id);
            if (feedback == null) return null;
            return _mapper.Map<FeedbackResponseDTO>(feedback); // Sử dụng AutoMapper
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