using BlinkayOccupation.Application.Models;
using BlinkayOccupation.Application.Services.StayPayment;
using BlinkayOccupation.Domain.Repositories.Installation;
using BlinkayOccupation.Domain.Repositories.Occupation;
using BlinkayOccupation.Domain.UnitOfWork;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BlinkayOccupation.Application.Services.Occupation
{
    public class OccupationsService : IOccupationsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOccupationRepository _occupationsRepository;
        private readonly IInstallationRepository _installationRepository;
        private readonly ILogger<OccupationsService> _logger;

        public OccupationsService(
            IUnitOfWork unitOfWork,
            IOccupationRepository occupationsRepository,
            IInstallationRepository installationRepository,
            ILogger<OccupationsService> logger)
        {
            _unitOfWork = unitOfWork;
            _occupationsRepository = occupationsRepository;
            _installationRepository = installationRepository;
            _logger = logger;
        }

        public async Task<List<CurrentParkingDataDto>> GetCurrentOccupation()
        {
            var list = new List<CurrentParkingDataDto>();
            try
            {
                _logger.LogInformation("Getting Current Occupation for today: {today}", DateTime.UtcNow.ToShortDateString());
                var installations = await _installationRepository.GetAllAsync(_unitOfWork.Context);
                var occupations = await _occupationsRepository.GetCurrentOccupations(_unitOfWork.Context, installations);

                foreach (var item in occupations)
                {
                    list.Add(new CurrentParkingDataDto
                    {
                        Date = item.Date ?? default,
                        Zone = new ZoneDto 
                        {
                            Id = item.ZoneId ?? string.Empty,
                            Value = item.Zone?.Name ?? string.Empty
                        },
                        Installation = new InstallationDto
                        {
                            Id = item.InstallationId ?? string.Empty,
                            Value = item.Installation?.Name ?? string.Empty
                        },
                        Tariff = new TariffDto
                        {
                            Id = item.TariffId ?? string.Empty,
                            Value = item.Tariff?.Name ?? string.Empty
                        },
                        Paid = item.PaidRealOccupation ?? 0,
                        Occupied = item.PaidRealOccupation ?? 0,
                        Sold = item.PaidOccupation ?? 0,
                        Total = item.Total ?? 0,
                        Unpaid = item.UnpaidRealOccupation ?? 0,
                        Available  = item.Total.HasValue && item.Total.Value > 0 ? item.Total.Value - (item.PaidRealOccupation.HasValue ? item.PaidRealOccupation.Value : 0) : 0,
                        OpenForSale = item.Total.HasValue && item.Total.Value > 0 ? item.Total.Value - (item.PaidOccupation.HasValue ? item.PaidOccupation.Value : 0) : 0,
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BlinkayOccupation-GetCurrentOccupation: An error occured when trying to get current occupation.");
                throw;
            }
            finally
            {
                await _unitOfWork.Context.DisposeAsync();
            }
        }
    }
}
