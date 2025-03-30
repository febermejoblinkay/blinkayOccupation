using BlinkayOccupation.Application.Services.StayPayment;
using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Repositories.Capacity;
using BlinkayOccupation.Domain.Repositories.Installation;
using BlinkayOccupation.Domain.Repositories.Occupation;
using BlinkayOccupation.Domain.Repositories.OccupationSnapshot;
using BlinkayOccupation.Domain.Repositories.Stay;
using BlinkayOccupation.Domain.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BlinkayOccupation.PaymentsWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string environmentVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(x => x.AddJsonFile("appsettings.json"))
                .ConfigureAppConfiguration(x => x.AddJsonFile($"appsettings.{environmentVariable}.json"))
                .ConfigureAppConfiguration(x => x.AddEnvironmentVariables())
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    services.AddDbContextFactory<BControlDbContext>(options =>
                    {
                        var connectionString = configuration.GetConnectionString("bControlDb");
                        options.UseNpgsql(connectionString);
                    });

                    services.AddScoped<IUnitOfWork, UnitOfWork>();
                    services.AddScoped<IStaysRepository, StaysRepository>();
                    services.AddScoped<IOccupationRepository, OccupationRepository>();
                    services.AddScoped<ICapacitiesRepository, CapacitiesRepository>();
                    services.AddScoped<IOccupationSnapshotRepository, OccupationSnapshotRepository>();
                    services.AddScoped<IInstallationRepository, InstallationRepository>();
                    services.AddScoped<IStayPaymentService, StayPaymentService>();
                    services.AddHostedService<PaymentProcessWorker>();
                })
                .UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration))
                .Build();

            host.Run();

        }
    }
}