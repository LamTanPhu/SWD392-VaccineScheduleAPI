using IRepositories.Entity.Schedules;
using IRepositories.IRepository;
using IRepositories.IRepository.Schedules;
using IServices.Interfaces.Schedules;
using ModelViews.Requests.Schedule;
using ModelViews.Responses.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Schedules
{
    public class VaccineReactionService : IVaccineReactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVaccineReactionRepository _repository;

        public VaccineReactionService(IUnitOfWork unitOfWork, IVaccineReactionRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<VaccineReactionResponseDTO> CreateReactionAsync(VaccineReactionRequestDTO request)
        {
            var reaction = new VaccineReaction
            {
                VaccinationScheduleId = request.VaccinationScheduleId,
                Reaction = request.Reaction,
                Severity = request.Severity,
                ReactionTime = request.ReactionTime,
                ResolvedTime = request.ResolvedTime
            };

            await _repository.InsertAsync(reaction);
            await _unitOfWork.SaveAsync();

            return new VaccineReactionResponseDTO
            {
                Id = reaction.Id,
                VaccinationScheduleId = reaction.VaccinationScheduleId,
                Reaction = reaction.Reaction,
                Severity = reaction.Severity,
                ReactionTime = reaction.ReactionTime,
                ResolvedTime = reaction.ResolvedTime,
                CreatedTime = reaction.CreatedTime,        
                LastUpdatedTime = reaction.LastUpdatedTime 
            };
        }       

        public async Task DeleteReactionAsync(string id)
        {
            var reaction = await _repository.GetByIdAsync(id);
            if (reaction == null)
                throw new KeyNotFoundException("Vaccine reaction not found.");

            await _repository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }

        public async Task<List<VaccineReactionResponseDTO>> GetAllReactionsAsync()
        {
            var reactions = await _repository.GetAllAsync();

            return reactions.Select(r => new VaccineReactionResponseDTO
            {
                Id = r.Id,
                VaccinationScheduleId = r.VaccinationScheduleId,
                Reaction = r.Reaction,
                Severity = r.Severity,
                ReactionTime = r.ReactionTime,
                ResolvedTime = r.ResolvedTime,
                CreatedTime = r.CreatedTime,
                LastUpdatedTime = r.LastUpdatedTime
            }).ToList();
        }


        public async Task<VaccineReactionResponseDTO> GetReactionByIdAsync(string id)
        {
            var reaction = await _repository.GetByIdAsync(id);

            if (reaction == null)
                throw new KeyNotFoundException("Vaccine reaction not found.");

            return new VaccineReactionResponseDTO
            {
                Id = reaction.Id,
                VaccinationScheduleId = reaction.VaccinationScheduleId,
                Reaction = reaction.Reaction,
                Severity = reaction.Severity,
                ReactionTime = reaction.ReactionTime,
                ResolvedTime = reaction.ResolvedTime,
                CreatedTime = reaction.CreatedTime,
                LastUpdatedTime = reaction.LastUpdatedTime
            };
        }


        public async Task UpdateReactionAsync(string id, VaccineReactionRequestDTO request)
        {
            var existingReaction = await _repository.GetByIdAsync(id);

            if (existingReaction == null)
                throw new KeyNotFoundException("Vaccine reaction not found.");

            // Update fields
            existingReaction.Reaction = request.Reaction;
            existingReaction.Severity = request.Severity;
            existingReaction.ReactionTime = request.ReactionTime;
            existingReaction.ResolvedTime = request.ResolvedTime;
            existingReaction.LastUpdatedTime = DateTimeOffset.UtcNow; // Update timestamp

            await _repository.UpdateAsync(existingReaction);
            await _unitOfWork.SaveAsync();
        }

    }
}
