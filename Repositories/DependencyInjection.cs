// Repositories/DependencyInjection.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Context;

namespace Repositories
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
            }

            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlServer(connectionString);
                // Nếu cần Lazy Loading
                // options.UseLazyLoadingProxies();
            });

            return services;
        }
        //public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        //{
        //    var connectionString = configuration.GetConnectionString("DefaultConnection");
        //    if (string.IsNullOrEmpty(connectionString))
        //    {
        //        throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
        //    }

        //    services.AddDbContext<DatabaseContext>(options =>
        //    {
        //        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        //        // Enable lazy loading here if needed:
        //        // options.UseLazyLoadingProxies();
        //    });

        //    return services;
        //}
    }
}