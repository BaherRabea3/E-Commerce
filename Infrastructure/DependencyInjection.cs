using Application.Common.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using Infrastructure.Identity;
using Infrastructure.Services.PaymentServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stripe;


namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {


            services.AddDbContext<AppDbContext>(options =>
                                                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")
                                                                    )
                                                .AddInterceptors(new SoftDeleteInterceptor())
                                              );
            services.AddDataProtection();

            services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole<int>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IAppDbContext, AppDbContext>();

            services.AddScoped<IFileService, Services.FileServices.FileService>();

            services.AddSingleton<StripeClient>(new StripeClient(
                configuration["Stripe:SecretKey"]));

            services.AddScoped<PaymentIntentService>();

            services.AddScoped<RefundService>();

            services.AddScoped<IPaymentGatewayService, StripePaymentGatewayService>();

            

            return services;
        }
    }
}
