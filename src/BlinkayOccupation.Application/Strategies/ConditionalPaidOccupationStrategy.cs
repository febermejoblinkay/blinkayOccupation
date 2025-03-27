using BlinkayOccupation.Domain.Models;
using BlinkayOccupation.Domain.Repositories.Capacity;
using BlinkayOccupation.Domain.Repositories.Occupation;

namespace BlinkayOccupation.Application.Strategies
{
    public class ConditionalPaidOccupationStrategy : BaseOccupationStrategy
    {
        public ConditionalPaidOccupationStrategy(
            IOccupationRepository occupationRepository, 
            ICapacitiesRepository capacitiesRepository)
        : base(occupationRepository, capacitiesRepository) { }

        protected override void ApplyOccupationChanges(Occupations occupation, Capacities? capacity, DateTime? paymentEndDate = null, Occupations? oldOccupation = null)
        {
            if (oldOccupation != null)
            {
                oldOccupation.UnpaidRealOccupation = (oldOccupation.UnpaidRealOccupation ?? 0) - 1;
            }

            if (paymentEndDate.HasValue && paymentEndDate.Value > DateTime.UtcNow)
            {
                occupation.PaidOccupation = (occupation.PaidOccupation ?? 0) + 1;
                occupation.Total = capacity != null ? capacity.Count : 0;

                //occupation.Total = (occupation.PaidOccupation ?? 0);
            }
        }
    }
}
