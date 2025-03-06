using IRepositories.IRepository;
using IRepositories.IRepository.Accounts;
using IRepositories.IRepository.Inventory;
using IRepositories.IRepository.Orders;
using IRepositories.IRepository.Schedules;
using IRepositories.IRepository.Vaccines;
using IServices.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Repository;
using Repositories.Repository.Accounts;
using Repositories.Repository.Inventory;
using Repositories.Repository.Order;
using Repositories.Repository.Schedules;
using Repositories.Repository.Vaccines;
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
            services.AddScoped<IVaccinePackageDetailsRepository, VaccinePackageDetailRepository>();
            services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
            services.AddScoped<IVaccineBatchRepository, VaccineBatchRepository>();
            services.AddScoped<IVaccineRepository, VaccineRepository>();
            services.AddScoped<IVaccineCategoryRepository, VaccineCategoryRepository>();
            services.AddScoped<IChildrenProfileRepository, ChildrenProfileRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IOrderPackageDetailsRepository, OrderPackageDetailsRepository>();
            services.AddScoped<IOrderVaccineDetailsRepository, OrderVaccineDetailsRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

        }
    }
}
