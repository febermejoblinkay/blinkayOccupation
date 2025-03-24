using BlinkayOccupation.API.Middlewares;
using BlinkayOccupation.API.Validators.Stays;
using BlinkayOccupation.Application.Services.Auth;
using BlinkayOccupation.Application.Services.Stay;
using BlinkayOccupation.Application.Strategies;
using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Repositories.Capacity;
using BlinkayOccupation.Domain.Repositories.Installation;
using BlinkayOccupation.Domain.Repositories.Occupation;
using BlinkayOccupation.Domain.Repositories.ParkingEvent;
using BlinkayOccupation.Domain.Repositories.ParkingRight;
using BlinkayOccupation.Domain.Repositories.Stay;
using BlinkayOccupation.Domain.Repositories.StayParkingRight;
using BlinkayOccupation.Domain.Repositories.User;
using BlinkayOccupation.Domain.Repositories.Users;
using BlinkayOccupation.Domain.Repositories.Zone;
using BlinkayOccupation.Domain.UnitOfWork;
using BlinkayOccupation.Infrastructure.Security;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
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
            new string[] {}
        }
    });
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

string environmentVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
builder.Configuration.AddJsonFile("appsettings.json");

if (!string.IsNullOrEmpty(environmentVariable))
{
    builder.Configuration.AddJsonFile($"appsettings.{environmentVariable}.json");
}

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDbContextFactory<BControlDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("bControlDb");
    options.UseNpgsql(connectionString); // Si usas PostgreSQL
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IStaysRepository, StaysRepository>();
builder.Services.AddScoped<IParkingEventsRepository, ParkingEventsRepository>();
builder.Services.AddScoped<IParkingRightsRepository, ParkingRightsRepository>();
builder.Services.AddScoped<IInstallationRepository, InstallationRepository>();
builder.Services.AddScoped<IZoneRepository, ZoneRepository>();
builder.Services.AddScoped<IStayParkingRightsRepository, StayParkingRightsRepository>();
builder.Services.AddScoped<IOccupationRepository, OccupationRepository>();
builder.Services.AddScoped<ICapacitiesRepository, CapacitiesRepository>();
builder.Services.AddScoped<IStayService, StayService>();
builder.Services.AddScoped<IOccupationStrategyFactory, OccupationStrategyFactory>();
builder.Services.AddScoped<IOccupationStrategy, EPNS_to_N_Strategy>();
builder.Services.AddScoped<IOccupationStrategy, ENPNS_to_N_Strategy>();
builder.Services.AddScoped<IOccupationStrategy, ENPNS_to_EPNS_Strategy>();
builder.Services.AddScoped<IOccupationStrategy, NEPNS_to_EPNS_Strategy>();
builder.Services.AddScoped<IOccupationStrategy, EPNS_to_EPS_Strategy>();
builder.Services.AddScoped<IOccupationStrategy, ENPNS_to_EPS_Strategy>();
builder.Services.AddScoped<IOccupationStrategy, ENPNS_to_ENPS_Strategy>();
builder.Services.AddScoped<IOccupationStrategy, ConditionalPaidOccupationStrategy>();

builder.Services.AddSingleton<IAccessTokenFactory>(sp =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    string jwtKey = jwtSettings["Key"] ?? throw new ArgumentNullException("Jwt:Key is missing!");
    string jwtIssuer = jwtSettings["Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer is missing!");

    return new AccessTokenFactory(jwtKey, jwtIssuer);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Obtén la configuración JWT del archivo de configuración
    var jwtSettings = builder.Configuration.GetSection("Jwt");

    //options.Audience = jwtSettings["Issuer"];
    //options.RequireAudience = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Issuer"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
        ValidateLifetime = true,
        RequireExpirationTime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("admin"));
    options.AddPolicy("User", policy => policy.RequireRole("admin", "user"));
    options.AddPolicy("Device", policy => policy.RequireRole("admin", "device"));
});

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// Deshabilitar HTTPS en contenedor
if (builder.Environment.IsDevelopment() && Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
{
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        serverOptions.ListenAnyIP(8081); // HTTP en puerto 8080
    });
    //builder.WebHost.UseKestrel(serverOptions =>
    //{
    //    serverOptions.ListenAnyIP(8081); // Escucha en todas las IPs del contenedor
    //});
}

builder.Services.AddValidatorsFromAssemblyContaining<AddStayRequestValidator>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseCors(x => x
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

app.MapControllers();

app.Run();
