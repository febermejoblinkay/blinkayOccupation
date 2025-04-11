using BlinkayOccupation.Application.Exceptions;
using BlinkayOccupation.Application.Models;
using BlinkayOccupation.Application.Services.StayPayment;
using BlinkayOccupation.Domain.Repositories.Installation;
using BlinkayOccupation.Domain.Repositories.Occupation;
using BlinkayOccupation.Domain.Repositories.Tariff;
using BlinkayOccupation.Domain.UnitOfWork;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BlinkayOccupation.Application.Services.Occupation
{
    public class OccupationsService : IOccupationsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOccupationRepository _occupationsRepository;
        private readonly IInstallationRepository _installationRepository;
        private readonly ITariffRepository _tariffRepository;
        private readonly ILogger<OccupationsService> _logger;

        public OccupationsService(
            IUnitOfWork unitOfWork,
            IOccupationRepository occupationsRepository,
            IInstallationRepository installationRepository,
            ITariffRepository tariffRepository,
            ILogger<OccupationsService> logger)
        {
            _unitOfWork = unitOfWork;
            _occupationsRepository = occupationsRepository;
            _installationRepository = installationRepository;
            _tariffRepository = tariffRepository;
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
                        Available = item.Total.HasValue && item.Total.Value > 0 ? item.Total.Value - (item.PaidRealOccupation.HasValue ? item.PaidRealOccupation.Value : 0) : 0,
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

        public async Task<List<OccupationDataByTarrifZone>> GetOccupationsByInstallation(string installationId)
        {
            try
            {
                _logger.LogInformation("Getting Occupation for installation: {installationId}", installationId);
                var list = new List<OccupationDataByTarrifZone>();
                var installation = await _installationRepository.GetByIdAsync(installationId, _unitOfWork.Context);
                if (installation is null)
                {
                    throw new InstallationNotFoundException();
                }

                var tariffsInstallation = await _tariffRepository.GetTariffByInsId(installationId, _unitOfWork.Context);
                var occupations = await _occupationsRepository.GetOccupationsByInstallation(installation, _unitOfWork.Context);

                foreach (var zone in installation.Zones)
                {
                    var obj = new OccupationDataByTarrifZone();
                    var tariffByZone = occupations.Where(x => x.ZoneId == zone.Id).ToList();
                    var totalPaidOccupation = tariffByZone.Sum(x => x.PaidOccupation) ?? 0;
                    var totalPaidRealOccupation = tariffByZone.Sum(x => x.PaidRealOccupation) ?? 0;
                    var totalUnpaid = tariffByZone.Sum(x => x.UnpaidRealOccupation) ?? 0;
                    var zoneTotal = totalPaidOccupation + totalPaidRealOccupation + totalUnpaid;
                    var capacityByZone = tariffByZone.Sum(x => x.Total);

                    foreach (var tariff in tariffsInstallation)
                    {
                        var tariffOccuObj = new OccupationByTariff();
                        var occupationTariff = occupations.Where(x => x.TariffId == tariff.Id && x.ZoneId == zone.Id).ToList();
                        obj.Zone = new ReferenceItem
                        {
                            Id = zone.Id,
                            Value = zone.Name
                        };

                        tariffOccuObj.TariffId = tariff.Id;
                        tariffOccuObj.TariffName = tariff.Name;

                        if (occupationTariff is not null && occupationTariff.Count > 0)
                        {
                            var sumPaidOccupation = occupationTariff.Sum(x => x.PaidOccupation) ?? 0;
                            var sumRealPaidOccupation = occupationTariff.Sum(x => x.PaidRealOccupation) ?? 0;
                            tariffOccuObj.PaidOccupation = sumPaidOccupation;
                            tariffOccuObj.RealOccupation = sumRealPaidOccupation;
                            tariffOccuObj.PaidOccupationPcg = FormatDecimal(sumPaidOccupation, totalPaidOccupation);
                            tariffOccuObj.RealOccupationPcg = FormatDecimal(sumRealPaidOccupation, totalPaidRealOccupation);
                            tariffOccuObj.Capacity = occupationTariff.Sum(x => x.Total) ?? 0;
                            obj.Tariff.Add(tariffOccuObj);
                        }
                        else
                        {
                            tariffOccuObj.PaidOccupation = 0;
                            tariffOccuObj.RealOccupation = 0;
                            tariffOccuObj.Capacity = 0;
                            tariffOccuObj.PaidOccupationPcg = FormatDecimal(0, totalPaidOccupation);
                            tariffOccuObj.RealOccupationPcg = FormatDecimal(0, totalPaidRealOccupation);
                            obj.Tariff.Add(tariffOccuObj);
                        }
                    }

                    obj.EntriesWithoutExitCount = totalUnpaid + totalPaidRealOccupation;
                    obj.TotalPaidOccupation = totalPaidOccupation;
                    obj.TotalRealOccupation = totalPaidRealOccupation;
                    obj.TotalPaidOccupationPcg = FormatDecimal(totalPaidOccupation, zoneTotal);
                    obj.TotalRealOccupationPcg = FormatDecimal(totalPaidRealOccupation, zoneTotal);
                    obj.TotalUnpaidOccupation = totalUnpaid;
                    obj.TotalUnpaidOccupationPcg = FormatDecimal(totalUnpaid, zoneTotal);
                    obj.Capacity = capacityByZone ?? 0;

                    list.Add(obj);
                }

                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BlinkayOccupation-GetOccupationsByInstallation: An error occured when trying to get occupation by installation.");
                throw;
            }
            finally
            {
                await _unitOfWork.Context.DisposeAsync();
            }
        }

        private string FormatDecimal(int totalPaidOccupation, int zoneTotal)
        {
            decimal value = 0m;
            if (zoneTotal > 0)
            {
                value = (decimal)totalPaidOccupation / (decimal)zoneTotal;
            }
            
            return value.ToString("P02", CultureInfo.InvariantCulture);
        }
    }
}
