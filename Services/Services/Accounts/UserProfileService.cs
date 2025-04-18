﻿using AutoMapper;
using IRepositories.Entity.Accounts;
using IRepositories.Enum;
using IRepositories.IRepository.Accounts;
using IServices.Interfaces.Accounts;
using Microsoft.AspNetCore.Http;
using ModelViews.Responses.Auth;
using ModelViews.Responses.ChildrenProfile;
using ModelViews.Responses.VaccineCenter;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Services.Services.Accounts
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public UserProfileService(
            IAccountRepository accountRepository,
            IJwtService jwtService,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ProfileResponseDTO?> GetProfileAsync()
        {
            var authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            Console.WriteLine($"Raw Authorization Header: '{authHeader}'");

            if (string.IsNullOrEmpty(authHeader))
                throw new UnauthorizedAccessException("Token is required.");

            var token = authHeader.Trim();
            var email = _httpContextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            Console.WriteLine($"Middleware Extracted email: '{email}'");
            if (string.IsNullOrEmpty(email))
                throw new UnauthorizedAccessException("Invalid token payload.");

            var expired = _jwtService.IsTokenExpired(token);
            Console.WriteLine($"Token Expired: {expired}, Expiration: {_jwtService.ExtractExpiration(token)}, Now: {DateTime.UtcNow}");
            if (expired)
                throw new UnauthorizedAccessException("Token has expired.");

            var account = await _accountRepository.GetByEmailAsync(email);
            if (account == null || account.DeletedTime != null)
                return null;

            var profile = _mapper.Map<ProfileResponseDTO>(account);
            Console.WriteLine($"Final Response DTO: Username={profile.Username}, Email={profile.Email}, Status={profile.Status}, Role={profile.Role}");
            return profile;
        }

        public async Task<Account?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email cannot be null or empty.");

            var account = await _accountRepository.GetByEmailAsync(email);
            return account; // Giữ nguyên trả về Account như cũ
        }

        public async Task<ProfileResponseDTO?> GetProfileByEmailAsync(string email)
        {
            var account = await _accountRepository.GetByEmailAsync(email);
            if (account == null || account.DeletedTime != null)
                return null;

            return MapToProfileResponseDTO(account);
        }

        public async Task<Account?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username), "Username cannot be null or empty.");

            return await _accountRepository.GetByUsernameAsync(username);
        }


        public async Task<Account?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email), "Email cannot be null or empty.");

            return await _accountRepository.GetByEmailAsync(email);
        }

        public async Task<Account?> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            if (string.IsNullOrEmpty(usernameOrEmail))
                throw new ArgumentNullException(nameof(usernameOrEmail), "Username or email cannot be null or empty.");

            return await _accountRepository.GetByUsernameOrEmailAsync(usernameOrEmail);
        }

        public async Task AddUserAsync(Account user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            await _accountRepository.InsertAsync(user);
            await _accountRepository.SaveAsync();
        }

        public async Task UpdateUserAsync(Account user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            await _accountRepository.UpdateAsync(user);
        }

        private ProfileResponseDTO MapToProfileResponseDTO(Account account)
        {
            return new ProfileResponseDTO
            {
                AccountId = account.Id,
                Username = account.Username,
                Email = account.Email,
                Role = account.Role.ToString(),
                Status = account.Status,
                VaccineCenter = account.VaccineCenter != null ? new VaccineCenterResponseDTO
                {
                    Id = account.VaccineCenter.Id,
                    Name = account.VaccineCenter.Name,
                    Location = account.VaccineCenter.Location,
                    ContactNumber = account.VaccineCenter.ContactNumber,
                    Email = account.VaccineCenter.Email,
                    Status = account.VaccineCenter.Status
                } : null,
                ChildrenProfiles = account.ChildrenProfiles?.Any() == true ? account.ChildrenProfiles.Select(cp => new ChildrenProfileResponseDTO
                {
                    Id = cp.Id,
                    AccountId = cp.AccountId,
                    FullName = cp.FullName,
                    DateOfBirth = cp.DateOfBirth,
                    Gender = cp.Gender,
                    Status = cp.Status,
                    Address = cp.Address
                }).ToList() : null
            };
        }
    }
}