using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories; // Add this to access Repositories.DependencyInjection
using Services;
using IServices.Interfaces.Accounts;
using IServices.Interfaces.Vaccines;
using IServices.Interfaces.Inventory;
using Services.Services.Accounts;
using Services.Services.Inventory;
using Services.Services.Vaccines;
using Core.Utils;
using IServices.Interfaces.Mail;
using Services.Services.Mail;
using IServices.Interfaces.Schedules;
using Services.Services.Schedules;
using IServices.Interfaces.Orders;
using Services.Services.Orders;

namespace VaccineScheduleAPI
{
    public static class DependencyInjection
    {
        public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabaseContext(configuration); // Call the repository layer's method
            services.AddRepositories();                 // Registers Repositories from Services Project
            services.AddServices(configuration);        // Registers Services (API-Specific)
        }

        private static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IVaccineCenterService, VaccineCenterService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IVaccinePackageService, VaccinePackageService>();
            services.AddScoped<IVaccinePackageDetailsService, VaccinePackageDetailsService>();
            services.AddScoped<IVaccineService, VaccineService>();
            services.AddScoped<IManufacturerService, ManufacturerService>();
            services.AddScoped<IVaccineBatchService, VaccineBatchService>();
            services.AddScoped<IFirebaseAuthService, FirebaseAuthService>();
            services.AddScoped<IVaccineCategoryService, VaccineCategoryService>();
            services.AddScoped<IChildrenProfileService, ChildrenProfileService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IOrderPackageDetailsService, OrderPackageDetailsServices>();
            services.AddScoped<IOrderVaccineDetailsService, OrderVaccineDetailsService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IVaccinationScheduleService, VaccinationScheduleService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IAccountAssignmentService, AccountAssignmentService>();
            services.AddScoped<IAccountUpdateService, AccountUpdateService>();
            services.AddScoped<IVaccineReactionService, VaccineReactionService>();
            services.AddScoped<IVaccineHistoryService, VaccineHistoryService>();

            // Register Email Settings
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            // Register Email Service
            services.AddTransient<IEmailService, EmailService>();
        }
    }
}