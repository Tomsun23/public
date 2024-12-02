using Clinic.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clinic.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) 
        {
            var connectionString = configuration["DbConnection"];

            services.AddDbContext<ClinicDbContext>(options => { 
                options.UseNpgsql(connectionString);
            });
            services.AddScoped<IClinicDbContext>(provider => provider.GetService<ClinicDbContext>());
            return services;
        }
    }
}
