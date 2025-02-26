using IRepositories.IRepository;
using IServices.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Repository;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddRepositories();
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IVaccineCenterRepository, VaccineCenterRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IVaccinePackageRepository, VaccinePackageRepository>();
            services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
            services.AddScoped<IVaccineBatchRepository, VaccineBatchRepository>();

        }
    }
}
