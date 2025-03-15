﻿using IRepositories.Entity.Accounts;
using IRepositories.IRepository.Accounts;
using IRepositories.IRepository;
using IServices.Interfaces.Accounts;
using ModelViews.Requests.ChildrenProfile;
using ModelViews.Responses.ChildrenProfile;

namespace Services.Services.Accounts
{
    public class ChildrenProfileService : IChildrenProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChildrenProfileRepository _repository;

        public ChildrenProfileService(IUnitOfWork unitOfWork, IChildrenProfileRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<IEnumerable<ChildrenProfileResponseDTO>> GetAllProfilesAsync()
        {
            var profiles = await _repository.GetAllAsync();
            return profiles.Select(p => new ChildrenProfileResponseDTO
            {
                Id = p.Id,
                AccountId = p.AccountId,
                FullName = p.FullName,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender,
                Status = p.Status,
                Address = p.Address
            }).ToList();
        }

        public async Task<IEnumerable<ChildrenProfileResponseDTO>> GetAllProfilesByAccountIdAsync(string accountId)
        {
            var profiles = await _repository.GetAllAsync();
            return profiles
                .Where(p => p.AccountId == accountId)
                .Select(p => new ChildrenProfileResponseDTO
                {
                    Id = p.Id,
                    AccountId = p.AccountId,
                    FullName = p.FullName,
                    DateOfBirth = p.DateOfBirth,
                    Gender = p.Gender,
                    Status = p.Status,
                    Address = p.Address
                }).ToList();
        }

        public async Task<ChildrenProfileResponseDTO?> GetProfileByIdAsync(string id)
        {
            var profile = await _repository.GetByIdAsync(id);
            if (profile == null) return null;
            return new ChildrenProfileResponseDTO
            {
                Id = profile.Id,
                AccountId = profile.AccountId,
                FullName = profile.FullName,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                Status = profile.Status,
                Address = profile.Address
            };
        }

        public async Task<ChildrenProfileResponseDTO> AddProfileAsync(string accountId, ChildrenProfileCreateUpdateDTO profileDto)
        {
            var profile = new ChildrenProfile
            {
                AccountId = accountId,
                FullName = profileDto.FullName,
                DateOfBirth = profileDto.DateOfBirth,
                Gender = profileDto.Gender,
                 Status = profileDto.Status,
                Address = profileDto.Address
            };
            await _repository.InsertAsync(profile);
            await _unitOfWork.SaveAsync();
            Console.WriteLine($"Saved profile: Id={profile.Id}, Address={profile.Address}");
            return new ChildrenProfileResponseDTO
            {
                Id = profile.Id,
                AccountId = profile.AccountId,
                FullName = profile.FullName,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                Status = profile.Status,
                Address = profile.Address
            };
        }

        public async Task UpdateProfileAsync(string id, ChildrenProfileCreateUpdateDTO profileDto)
        {
            var existingProfile = await _repository.GetByIdAsync(id);
            if (existingProfile == null)
                throw new Exception("Children profile not found.");

            existingProfile.FullName = profileDto.FullName;
            existingProfile.DateOfBirth = profileDto.DateOfBirth;
            existingProfile.Gender = profileDto.Gender;
            existingProfile.Status = profileDto.Status;
            existingProfile.Address = profileDto.Address; // Ensure this updates
            await _repository.UpdateAsync(existingProfile);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteProfileAsync(string id)
        {
            await _repository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }
    }
}
