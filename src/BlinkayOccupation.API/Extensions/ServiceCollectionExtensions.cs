using BlinkayOccupation.Application.Services.Auth;
using BlinkayOccupation.Application.Services.Occupation;
using BlinkayOccupation.Application.Services.ParkingEvent;
using BlinkayOccupation.Application.Services.Stay;
using BlinkayOccupation.Application.Strategies;
using BlinkayOccupation.Domain.Repositories.Attachment;
using BlinkayOccupation.Domain.Repositories.Capacity;
using BlinkayOccupation.Domain.Repositories.InputDevice;
using BlinkayOccupation.Domain.Repositories.Installation;
using BlinkayOccupation.Domain.Repositories.Occupation;
using BlinkayOccupation.Domain.Repositories.ParkingEvent;
using BlinkayOccupation.Domain.Repositories.ParkingRight;
using BlinkayOccupation.Domain.Repositories.Space;
using BlinkayOccupation.Domain.Repositories.Stay;
using BlinkayOccupation.Domain.Repositories.StayParkingRight;
using BlinkayOccupation.Domain.Repositories.StreetSection;
using BlinkayOccupation.Domain.Repositories.Tariff;
using BlinkayOccupation.Domain.Repositories.User;
using BlinkayOccupation.Domain.Repositories.Users;
using BlinkayOccupation.Domain.Repositories.VehicleEvent;
using BlinkayOccupation.Domain.Repositories.Zone;
using BlinkayOccupation.Domain.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BlinkayOccupation.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCustomRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IStaysRepository, StaysRepository>();
            services.AddScoped<IParkingEventsRepository, ParkingEventsRepository>();
            services.AddScoped<IParkingRightsRepository, ParkingRightsRepository>();
            services.AddScoped<IInstallationRepository, InstallationRepository>();
            services.AddScoped<IZoneRepository, ZoneRepository>();
            services.AddScoped<IStayParkingRightsRepository, StayParkingRightsRepository>();
            services.AddScoped<IOccupationRepository, OccupationRepository>();
            services.AddScoped<ICapacitiesRepository, CapacitiesRepository>();
            services.AddScoped<IInputDeviceRepository, InputDeviceRepository>();
            services.AddScoped<IVehicleEventsRepository, VehicleEventsRepository>();
            services.AddScoped<ITariffRepository, TariffRepository>();
            services.AddScoped<IZoneRepository, ZoneRepository>();
            services.AddScoped<IStreetSectionRepository, StreetSectionRepository>();
            services.AddScoped<ISpaceRepository, SpaceRepository>();
            services.AddScoped<IAttachmentRepository, AttachmentRepository>();
        }

        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IStayService, StayService>();
            services.AddScoped<IParkingEventsService, ParkingEventsService>();
            services.AddScoped<IOccupationsService, OccupationsService>();
            services.AddScoped<IOccupationStrategyFactory, OccupationStrategyFactory>();
            services.AddScoped<IOccupationStrategy, EPNS_to_N_Strategy>();
            services.AddScoped<IOccupationStrategy, ENPNS_to_N_Strategy>();
            services.AddScoped<IOccupationStrategy, ENPNS_to_EPNS_Strategy>();
            services.AddScoped<IOccupationStrategy, NEPNS_to_EPNS_Strategy>();
            services.AddScoped<IOccupationStrategy, EPNS_to_EPS_Strategy>();
            services.AddScoped<IOccupationStrategy, ENPNS_to_EPS_Strategy>();
            services.AddScoped<IOccupationStrategy, ENPNS_to_ENPS_Strategy>();
            services.AddScoped<IOccupationStrategy, ConditionalPaidOccupationStrategy>();
        }

        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            string jwtKey = jwtSettings["Key"];
            string jwtIssuer = jwtSettings["Issuer"];

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtIssuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        public static void AddCustomAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("admin"));
                options.AddPolicy("User", policy => policy.RequireRole("admin", "user"));
                options.AddPolicy("Device", policy => policy.RequireRole("admin", "device"));
            });
        }

        public static void AddSwaggerGenConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter the JWT token in the format 'Bearer {token}'"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
            });
        }
    }
}
