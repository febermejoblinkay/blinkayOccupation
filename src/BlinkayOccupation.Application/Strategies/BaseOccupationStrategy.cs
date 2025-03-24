using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;
using BlinkayOccupation.Domain.Repositories.Capacity;
using BlinkayOccupation.Domain.Repositories.Occupation;

namespace BlinkayOccupation.Application.Strategies
{
    public abstract class BaseOccupationStrategy : IOccupationStrategy
    {
        protected readonly IOccupationRepository _occupationRepository;
        protected readonly ICapacitiesRepository _capacitiesRepository;

        protected BaseOccupationStrategy(IOccupationRepository occupationRepository, ICapacitiesRepository capacitiesRepository)
        {
            _occupationRepository = occupationRepository;
            _capacitiesRepository = capacitiesRepository;
        }

        public async Task ExecuteAsync(
            Stays stay,
            string? tariffId,
            string oldState,
            string newState,
            BControlDbContext context,
            DateTime? paymentEndDate = null)
        {
            if (!stay.EntryDate.HasValue && !stay.ExitDate.HasValue && !paymentEndDate.HasValue)
                return;

            var hasBeenInitialized = false;
            var date = stay.EntryDate.HasValue ? stay.EntryDate.Value.Date : (stay.ExitDate.HasValue ? stay.ExitDate.Value.Date : DateTime.UtcNow.Date);
            var zoneId = stay.ZoneId;
            var installationId = stay.InstallationId;
            var existingOccupations = await _occupationRepository.GetOccupationsAvailable(date, installationId, zoneId, tariffId, context);
            var capacity = await _capacitiesRepository.GetAvailableCapacities(installationId, zoneId, tariffId, stay.EntryDate, stay.ExitDate, context);
            Occupations occupation = null;

            if (existingOccupations?.Count == 0)
            {
                occupation = await CreateOccupationObj(tariffId, context, date, zoneId, installationId);
                //hasBeenInitialized = true;
                ApplyOccupationChanges(occupation, capacity, paymentEndDate);
            }
            else if (existingOccupations.Count == 2)
            {
                foreach (var currentOccupation in existingOccupations)
                {
                    //Si viene de una estancia creada que ya tiene una tarifa asociada no decrementarlo
                    if (string.IsNullOrWhiteSpace(currentOccupation?.TariffId) && !string.IsNullOrWhiteSpace(tariffId))
                    {
                        currentOccupation.UnpaidRealOccupation = (currentOccupation.UnpaidRealOccupation ?? 0) - 1;
                        await _occupationRepository.UpdateAsync(currentOccupation, context);
                    }
                    else
                    {
                        ApplyOccupationChanges(currentOccupation, capacity, paymentEndDate);
                    }
                }
            }
            else //if (existingOccupations.Count == 1 && existingOccupations.Any(x => !string.IsNullOrWhiteSpace(x.TariffId)))
            {
                if (existingOccupations?.Count > 0)
                {
                    ApplyOccupationChanges(existingOccupations.FirstOrDefault(), capacity, paymentEndDate);
                }
            }

            //if (!hasBeenInitialized && occupation != null && string.IsNullOrEmpty(occupation?.TariffId) && !string.IsNullOrWhiteSpace(tariffId))
            //{
            //    occupation.UnpaidRealOccupation = (occupation.UnpaidRealOccupation ?? 0) - 1;
            //    occupation.Total = capacity != null ? capacity.Count : 0;
            //    await _occupationRepository.UpdateAsync(occupation, context);

            //    occupation = await CreateOccupationObj(tariffId, context, date, zoneId, installationId);
            //}

            //ApplyOccupationChanges(occupation, capacity, paymentEndDate);

            await context.SaveChangesAsync();
        }

        private async Task<Occupations?> CreateOccupationObj(string? tariffId, BControlDbContext context, DateTime date, string zoneId, string installationId)
        {
            Occupations? occupation = new Occupations
            {
                Id = Guid.CreateVersion7().ToString(),
                Date = date,
                InstallationId = !string.IsNullOrWhiteSpace(installationId) ? installationId : null,
                ZoneId = !string.IsNullOrWhiteSpace(zoneId) ? zoneId : null,
                TariffId = !string.IsNullOrWhiteSpace(tariffId) ? tariffId : null,
                PaidRealOccupation = 0,
                UnpaidRealOccupation = 0,
                PaidOccupation = 0,
                Total = 0
            };
            await _occupationRepository.AddAsync(occupation, context);
            return occupation;
        }

        protected abstract void ApplyOccupationChanges(Occupations occupation, Capacities? capacity, DateTime? paymentEndDate = null);
    }
}
