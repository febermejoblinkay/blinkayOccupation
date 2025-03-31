using BlinkayOccupation.API.Extensions;
using BlinkayOccupation.API.Middlewares;
using BlinkayOccupation.API.Validators.Stays;
using BlinkayOccupation.Application.Models;
using BlinkayOccupation.Application.Services.AzureBlob;
using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Infrastructure.Security;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenConfiguration();

builder.Configuration.AddAppSettings(builder);

builder.Services.AddDbContextFactory<BControlDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("bControlDb");
    options.UseNpgsql(connectionString);
});

IConfiguration configuration = builder.Configuration.GetSection("AzureBlobStorage");
builder.Services.Configure<BlobStorageSettings>(configuration);
builder.Services.AddSingleton<IBlobStorage, AzureBlobStorage>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<AzureBlobStorage>>();
    var config = provider.GetRequiredService<IConfiguration>();
    var settings = config.GetSection("AzureBlobStorage").Get<BlobStorageSettings>();
    var blobStorage = new AzureBlobStorage(logger);
    blobStorage.SetEndpoint(settings.BlobEndpoint);

    if (!blobStorage.SetContainer(settings.ContainerName))
    {
        throw new Exception("Failed to configure Blob Container.");
    }

    return blobStorage;
});

builder.Services.AddCustomRepositories();
builder.Services.AddCustomServices();
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
builder.Services.AddSingleton<IAccessTokenFactory>(sp =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    string jwtKey = jwtSettings["Key"] ?? throw new ArgumentNullException("Jwt:Key is missing!");
    string jwtIssuer = jwtSettings["Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer is missing!");

    return new AccessTokenFactory(jwtKey, jwtIssuer);
});

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCustomAuthorization();

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

builder.Services.AddValidatorsFromAssemblyContaining<CreateVehicleParkingRequestValidator>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || Environment.GetEnvironmentVariable("ENABLE_SWAGGER") == "true")
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
