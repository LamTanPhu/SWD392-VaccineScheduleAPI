using IRepositories.Entity.Schedules;
using IRepositories.IRepository;
using IRepositories.IRepository.Schedules;
using IServices.Interfaces.Schedules;
using ModelViews.Requests.History;
using ModelViews.Responses.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Schedules
{
    public class VaccineHistoryService : IVaccineHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVaccineHistoryRepository _repository;

        public VaccineHistoryService(IUnitOfWork unitOfWork, IVaccineHistoryRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<IEnumerable<VaccineHistoryResponseDTO>> GetAllAsync()
        {
            var histories = await _repository.GetAllAsync();
            return histories.Select(h => new VaccineHistoryResponseDTO
            {
                Id = h.Id,
                VaccineId = h.VaccineId,
                ProfileId = h.ProfileId,
                AccountId = h.AccountId,
                CenterId = h.CenterId,
                AdministeredDate = h.AdministeredDate,
                AdministeredBy = h.AdministeredBy,
                DocumentationProvided = h.DocumentationProvided,
                Notes = h.Notes,
                VerifiedStatus = h.VerifiedStatus,
                VaccinedStatus = h.VaccinedStatus,
                DosedNumber = h.DosedNumber
            }).ToList();
        }

        public async Task<IEnumerable<VaccineHistoryResponseDTO>> GetAllByUserIdAsync(string userId)
        {
            var histories = await _repository.GetByUserIdAsync(userId);
            return histories.Select(h => new VaccineHistoryResponseDTO
            {
                Id = h.Id,
                VaccineId = h.VaccineId,
                ProfileId = h.ProfileId,
                AccountId = h.AccountId,
                CenterId = h.CenterId,
                AdministeredDate = h.AdministeredDate,
                AdministeredBy = h.AdministeredBy,
                DocumentationProvided = h.DocumentationProvided,
                Notes = h.Notes,
                VerifiedStatus = h.VerifiedStatus,
                VaccinedStatus = h.VaccinedStatus,
                DosedNumber = h.DosedNumber
            }).ToList();
        }

        public async Task<IEnumerable<VaccineHistoryResponseDTO>> SearchByDateAsync(string userId, DateTime date)
        {
            var histories = await _repository.SearchByDateAsync(userId, date);
            return histories.Select(h => new VaccineHistoryResponseDTO
            {
                Id = h.Id,
                VaccineId = h.VaccineId,
                ProfileId = h.ProfileId,
                AccountId = h.AccountId,
                CenterId = h.CenterId,
                AdministeredDate = h.AdministeredDate,
                AdministeredBy = h.AdministeredBy,
                DocumentationProvided = h.DocumentationProvided,
                Notes = h.Notes,
                VerifiedStatus = h.VerifiedStatus,
                VaccinedStatus = h.VaccinedStatus,
                DosedNumber = h.DosedNumber
            }).ToList();
        }

        public async Task<IEnumerable<VaccineHistoryResponseDTO>> SearchByCenterIdAsync(string centerId)
        {
            var histories = await _repository.SearchByCenterIdAsync(centerId);
            return histories.Select(h => new VaccineHistoryResponseDTO
            {
                Id = h.Id,
                VaccineId = h.VaccineId,
                ProfileId = h.ProfileId,
                AccountId = h.AccountId,
                CenterId = h.CenterId,
                AdministeredDate = h.AdministeredDate,
                AdministeredBy = h.AdministeredBy,
                DocumentationProvided = h.DocumentationProvided,
                Notes = h.Notes,
                VerifiedStatus = h.VerifiedStatus,
                VaccinedStatus = h.VaccinedStatus,
                DosedNumber = h.DosedNumber
            }).ToList();
        }

        public async Task CreateAsync(VaccineHistoryRequestDTO requestDto)
        {
            var history = new VaccineHistory
            {
                VaccineId = requestDto.VaccineId,
                ProfileId = requestDto.ProfileId,
                AccountId = requestDto.AccountId,
                CenterId = requestDto.CenterId,
                AdministeredDate = requestDto.AdministeredDate,
                AdministeredBy = requestDto.AdministeredBy,
                DocumentationProvided = requestDto.DocumentationProvided,
                Notes = requestDto.Notes,
                VerifiedStatus = requestDto.VerifiedStatus,
                VaccinedStatus = requestDto.VaccinedStatus,
                DosedNumber = requestDto.DosedNumber
            };
            await _repository.InsertAsync(history);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(string id, VaccineHistoryRequestDTO requestDto)
        {
            var existingHistory = await _repository.GetByIdAsync(id);
            if (existingHistory == null)
                throw new Exception("Vaccine history not found.");

            existingHistory.VaccineId = requestDto.VaccineId;
            existingHistory.ProfileId = requestDto.ProfileId;
            existingHistory.AccountId = requestDto.AccountId;
            existingHistory.CenterId = requestDto.CenterId;
            existingHistory.AdministeredDate = requestDto.AdministeredDate;
            existingHistory.AdministeredBy = requestDto.AdministeredBy;
            existingHistory.DocumentationProvided = requestDto.DocumentationProvided;
            existingHistory.Notes = requestDto.Notes;
            existingHistory.VerifiedStatus = requestDto.VerifiedStatus;
            existingHistory.VaccinedStatus = requestDto.VaccinedStatus;
            existingHistory.DosedNumber = requestDto.DosedNumber;

            await _repository.UpdateAsync(existingHistory);
            await _unitOfWork.SaveAsync();
        }
    }
}