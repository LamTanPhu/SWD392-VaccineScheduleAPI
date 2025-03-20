using AutoMapper;
using IRepositories.Entity.Accounts;
using IRepositories.IRepository.Accounts;
using IServices.Interfaces.Accounts;
using ModelViews.Requests.Auth;
using ModelViews.Responses.Auth;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Services.Services.Accounts
{
    public class AccountUpdateService : IAccountUpdateService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public AccountUpdateService(
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

        public async Task<ProfileResponseDTO?> UpdateAccountAsync(UpdateAccountRequestDTO request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Update request cannot be null.");

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

            _mapper.Map(request, account);
            await _accountRepository.UpdateAsync(account);
            await _accountRepository.SaveAsync();

            return _mapper.Map<ProfileResponseDTO>(account);
        }
    }
}