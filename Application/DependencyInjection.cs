
using Application.Behaviours;
using Application.Common.Interfaces;
using FluentValidation;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly);
            });
            services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            services.AddHangfire(option =>
            {
                option.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddHangfireServer();

            return services;
        }
    }
}
