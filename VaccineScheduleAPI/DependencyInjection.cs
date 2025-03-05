using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Context;
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

namespace VaccineScheduleAPI
{
    public static class DependencyInjection
    {
        public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabase(configuration);  // Registers MySQL Database
            services.AddInfrastructure();  // Registers Repositories from Service Project
            services.AddServices(configuration);        // Registers Services (API-Specific)
        }

        private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            // Registering DbContext with Pomelo MySQL provider and enabling lazy loading
            services.AddDbContext<DatabaseContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("The connection string 'DefaultConnection' is missing.");
                }

                options.UseLazyLoadingProxies() // Enable Lazy Loading
                       .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
        }

        private static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IVaccineCenterService, VaccineCenterService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IVaccinePackageService, VaccinePackageService>();
            services.AddScoped<IVaccinePackageDetailsService, VaccinePackageDetailsService>();
            services.AddScoped<IManufacturerService, ManufacturerService>();
            services.AddScoped<IVaccineBatchService, VaccineBatchService>();
            services.AddScoped<IFirebaseAuthService, FirebaseAuthService>();

            // Register Email Settings
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            // Register Email Service
            services.AddTransient<IEmailService, EmailService>();
        }
    }
}
