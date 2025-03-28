using BlinkayOccupation.Application.Services.StayPayment;
using BlinkayOccupation.PaymentsWorker.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlinkayOccupation.PaymentsWorker
{
    public class PaymentProcessWorker : BackgroundService
    {
        private readonly ILogger<PaymentProcessWorker> _logger;
        private string _hostname;
        private string _environment;
        private AppSettings _appSettings;

        private readonly IStayPaymentService _stayPaymentService;

        public PaymentProcessWorker(
            ILogger<PaymentProcessWorker> logger,
            IOptions<AppSettings> appSettings,
            IStayPaymentService stayPaymentService)
        {
            _hostname = Environment.GetEnvironmentVariable("HOSTNAME");
            _environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            _logger = logger;
            _appSettings = appSettings.Value;
            _stayPaymentService = stayPaymentService;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PaymentProcessWorker-{environment}: {hostname}::Start", _environment, _hostname);

            var paymentsTask = ProcessInitEndPaymentsEachMinute(stoppingToken);
            var snapshotTask = ProcessSnapshot(stoppingToken);
            var cloneTask = CloneRealOccupation(stoppingToken);

            //await Task.WhenAll(paymentsTask, snapshotTask, cloneTask);
            await Task.WhenAll(paymentsTask);
        }

        private async Task ProcessInitEndPaymentsEachMinute(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _logger.LogInformation("PaymentProcessWorker-{environment}: ");
                await _stayPaymentService.ProcessInitEndPaymentStay();
                DateTime nextMinute = DateTime.Now.AddMinutes(1);
                nextMinute = new DateTime(nextMinute.Year, nextMinute.Month, nextMinute.Day, nextMinute.Hour, nextMinute.Minute, 0);
                TimeSpan delay = nextMinute - DateTime.Now;
                await Task.Delay(delay, token);
            }
        }

        private async Task ProcessSnapshot(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                DateTime ahora = DateTime.Now;
                if (ahora.Minute == 15)
                {
                }

                DateTime nextRun = DateTime.Now.AddHours(1);
                nextRun = new DateTime(nextRun.Year, nextRun.Month, nextRun.Day, nextRun.Hour, 15, 0);
                TimeSpan delay = nextRun - DateTime.Now;
                await Task.Delay(delay, token);
            }
        }

        private async Task CloneRealOccupation(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                DateTime ahora = DateTime.Now;
                if (ahora.Hour == 0 && ahora.Minute == 0)
                {
                }

                DateTime nextRun = DateTime.Today.AddDays(1);
                TimeSpan delay = nextRun - DateTime.Now;
                await Task.Delay(delay, token);
            }
        }
    }
}
