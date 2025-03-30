using BlinkayOccupation.Application.Services.StayPayment;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlinkayOccupation.PaymentsWorker
{
    public class PaymentProcessWorker : BackgroundService
    {
        private readonly ILogger<PaymentProcessWorker> _logger;
        private string _environment;

        private readonly IStayPaymentService _stayPaymentService;

        public PaymentProcessWorker(
            ILogger<PaymentProcessWorker> logger,
            IStayPaymentService stayPaymentService)
        {
            _environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            _logger = logger;
            _stayPaymentService = stayPaymentService;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PaymentProcessWorker-{environment}:Start", _environment);

            try
            {
                var paymentTask = ProcessInitEndPaymentsEachMinute(stoppingToken);
                var snapshotTask = ProcessSnapshot(stoppingToken);
                var cloneTask = CloneRealOccupationForAllInstallations(stoppingToken);

                await Task.WhenAll(paymentTask, snapshotTask, cloneTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentProcessWorker-{environment}:An error has occured when processing worker.", _environment);
            }
        }

        private async Task ProcessInitEndPaymentsEachMinute(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("PaymentProcessWorker-{environment}: Processing payments...", _environment);
                    await _stayPaymentService.ProcessInitEndPaymentStay();

                    DateTime nextMinute = DateTime.Now.AddMinutes(1);
                    nextMinute = new DateTime(nextMinute.Year, nextMinute.Month, nextMinute.Day, nextMinute.Hour, nextMinute.Minute, 0);
                    TimeSpan delay = nextMinute - DateTime.Now;
                    _logger.LogInformation("Next payment task scheduled for: {nextMinute}", nextMinute);

                    await Task.Delay(delay, token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error has occured when trying to process init and end payment data in {environment}", _environment);
                }
            }
        }

        private async Task ProcessSnapshot(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    DateTime now = DateTime.Now;
                    _logger.LogInformation("PaymentProcessWorker-{environment}: Processing snapshot... Actual time: {now}", _environment, now);

                    await _stayPaymentService.ProcessOccupationsSnapshot();

                    _logger.LogInformation("Next snapshot task scheduled in 15 minutes...");
                    await Task.Delay(TimeSpan.FromMinutes(15), token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error has occured when trying to take snapshot of occupation in {environment}", _environment);
                }
            }
        }

        private async Task CloneRealOccupationForAllInstallations(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var installations = await _stayPaymentService.GetAllInstallationsAsync();

                    var tasks = new List<Task>();

                    foreach (var installation in installations)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            DateTime now = installation.DateTimeNow();
                            DateTime tomorrow = now.Date.AddDays(1);
                            DateTime nextRunTime = tomorrow.AddMinutes(5);

                            if (nextRunTime < now)
                            {
                                nextRunTime = nextRunTime.AddDays(1);
                            }

                            TimeSpan delay = nextRunTime - now;

                            _logger.LogInformation("Next real occupation clone task for Installation {installationId} scheduled for: {nextMidnight}", installation.Id, nextRunTime);

                            await Task.Delay(delay, token);

                            _logger.LogInformation("Clonando ocupación real para la instalación {installationId}...", installation.Id);
                            await _stayPaymentService.CloneOccupationForInstallation(installation);
                        }));
                    }

                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error has occured when trying to clone real occupation in {environment}", _environment);
                }
            }
        }
    }
}
