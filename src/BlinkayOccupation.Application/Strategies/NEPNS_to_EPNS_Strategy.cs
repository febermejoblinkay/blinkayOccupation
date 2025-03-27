using BlinkayOccupation.Domain.Models;
using BlinkayOccupation.Domain.Repositories.Capacity;
using BlinkayOccupation.Domain.Repositories.Occupation;

namespace BlinkayOccupation.Application.Strategies
{
    public class NEPNS_to_EPNS_Strategy : BaseOccupationStrategy
    {
        public NEPNS_to_EPNS_Strategy(
            IOccupationRepository occupationRepository,
            ICapacitiesRepository capacitiesRepository)
            : base(occupationRepository, capacitiesRepository) { }

        protected override void ApplyOccupationChanges(Occupations occupation, Capacities? capacity, DateTime? paymentEndDate = null, Occupations? oldOccupation = null)
        {
            occupation.PaidRealOccupation = (occupation.PaidRealOccupation ?? 0) + 1;
            occupation.Total = capacity != null ? capacity.Count : 0;
            //occupation.Total = (occupation.PaidRealOccupation ?? 0) + (occupation.UnpaidRealOccupation ?? 0);
        }
    }
}
