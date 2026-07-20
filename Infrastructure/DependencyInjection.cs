using Application.Common.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using Infrastructure.Identity;
using Infrastructure.Services.AuthServices;
using Infrastructure.Services.PaymentServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.Text;


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

            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
                .AddRoles<IdentityRole<int>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
          .AddJwtBearer(options =>
          {
              options.TokenValidationParameters = new TokenValidationParameters()
              {
                  ValidateIssuer = true,
                  ValidIssuer = configuration["Jwt:Issuer"],
                  ValidateAudience = true,
                  ValidAudience =configuration["Jwt:Audience"],
                  ValidateLifetime = true,
                  ValidateIssuerSigningKey = true,
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
              };
          });


            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IAuthService, AuthService>();

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
