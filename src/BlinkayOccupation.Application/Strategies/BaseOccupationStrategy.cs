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
            BControlDbContext context)
        {
            if (stay.EntryEvent == null && stay.ExitEvent == null && stay is null)
                return;

            var date = stay.Installation.DateTimeNow();
            var zoneId = stay.ZoneId;
            var installationId = stay.InstallationId;
            var oldStayHasntPayment = oldState.Split(',')[1] == "NP";
            var newStayHasPayment = !newState.Equals("N") ? newState.Split(',')[1] == "P" : false;
            var isNewStay = newState.Equals("N");
            var existingOccupations = await _occupationRepository.GetOccupationsAvailable(date, installationId, zoneId, tariffId, context);
            var capacity = await _capacitiesRepository.GetAvailableCapacities(installationId, zoneId, tariffId, stay.EntryDate, stay.ExitDate, context);

            if (existingOccupations is null || !existingOccupations.Any())
            {
                var occupation = await CreateOccupationObj(tariffId, context, date, zoneId, installationId);
                ApplyOccupationChanges(stay, occupation, capacity);
            }
            else if (existingOccupations.Count == 2)
            {
                await HandleTwoOccupations(stay, existingOccupations, oldStayHasntPayment, newStayHasPayment, capacity, context, isNewStay);
            }
            else if (existingOccupations.Count == 1 && oldStayHasntPayment && newStayHasPayment)
            {
                var occupation = await CreateOccupationObj(tariffId, context, date, zoneId, installationId);
                ApplyOccupationChanges(stay, occupation, capacity, existingOccupations.ElementAt(0));
                await _occupationRepository.UpdateAsync(existingOccupations.ElementAt(0), context);
            }
            else
            {
                ApplyOccupationChanges(stay, existingOccupations.First(), capacity);
            }

            await context.SaveChangesAsync();
        }

        private async Task HandleTwoOccupations(
            Stays stay,
            List<Occupations> existingOccupations,
            bool oldStayHasntPayment,
            bool newStayHasPayment,
            Capacities? capacity,
            BControlDbContext context, 
            bool isNewStay)
        {
            var occupationWithTariff = existingOccupations.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.TariffId));
            var occupationWithoutTariff = existingOccupations.FirstOrDefault(x => string.IsNullOrWhiteSpace(x.TariffId));

            if (oldStayHasntPayment && newStayHasPayment)
            {
                ApplyOccupationChanges(stay, occupationWithTariff, capacity, occupationWithoutTariff);
                await _occupationRepository.UpdateRangeAsync(new List<Occupations> { occupationWithTariff, occupationWithoutTariff }, context);
            }
            else
            {
                ApplyOccupationChanges(stay, occupationWithTariff, capacity);
                await _occupationRepository.UpdateAsync(occupationWithTariff, context);
            }
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

        protected abstract void ApplyOccupationChanges(Stays stay, Occupations occupation, Capacities? capacity, Occupations? oldOccupation = null);
    }
}
