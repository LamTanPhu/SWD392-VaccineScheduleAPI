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

namespace VaccineScheduleAPI
{
    public static class DependencyInjection
    {
        public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabase(configuration);  // Registers MySQL Database
            services.AddInfrastructure();  // Registers Repositories from Service Project
            services.AddServices();        // Registers Services (API-Specific)
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

        private static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IVaccineCenterService, VaccineCenterService>(); //  Register Services 
            services.AddScoped<IAccountService, AccountService>(); //  Register Services 
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IVaccinePackageService, VaccinePackageService>();
            services.AddScoped<IManufacturerService, ManufacturerService>();
            services.AddScoped<IVaccineBatchService, VaccineBatchService>();

        }
    }
}
