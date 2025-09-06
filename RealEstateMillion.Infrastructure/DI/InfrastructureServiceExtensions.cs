using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RealEstateMillion.Application.Services.Implementations;
using RealEstateMillion.Application.Services.Interfaces;
using RealEstateMillion.Domain.Interfaces;
using RealEstateMillion.Infrastructure.Data.Context;
using RealEstateMillion.Infrastructure.Data.Repositories;
using RealEstateMillion.Infrastructure.Data.UnitOfWork;

namespace RealEstateMillion.Infrastructure.DI
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IPropertyRepository, PropertyRepository>();
            services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();
            services.AddScoped<IPropertyTraceRepository, PropertyTraceRepository>();
            services.AddScoped<IOwnerRepository, OwnerRepository>();
            services.AddScoped<IPropertyService, PropertyService>();
            services.AddScoped<IPropertyImageService, PropertyImageService>();

            return services;
        }
        public static IServiceCollection AddSwagg(this IServiceCollection services)
        {

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Real Estate API",
                    Version = "v1",
                    Description = "Api Rest Million - ROINER GOMEZ"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }

                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
            });

            services.AddHealthChecks()
                .AddDbContextCheck<RealEstateMillionDbContext>("database")
                .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

            return services;
        }
    }
}
