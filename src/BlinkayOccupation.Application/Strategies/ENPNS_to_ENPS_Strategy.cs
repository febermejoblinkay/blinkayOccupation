using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;
using BlinkayOccupation.Domain.Repositories.Capacity;
using BlinkayOccupation.Domain.Repositories.Occupation;

namespace BlinkayOccupation.Application.Strategies
{
    public class ENPNS_to_ENPS_Strategy : BaseOccupationStrategy
    {
        public ENPNS_to_ENPS_Strategy(
            IOccupationRepository occupationRepository,
            ICapacitiesRepository capacitiesRepository)
            : base(occupationRepository, capacitiesRepository) { }

        protected override void ApplyOccupationChanges(Stays stay, Occupations occupation, Capacities? capacity, Occupations? oldOccupation = null)
        {
            occupation.UnpaidRealOccupation = (occupation.UnpaidRealOccupation ?? 0) > 0 ? occupation.UnpaidRealOccupation - 1 : 0;
            occupation.Total = capacity != null ? capacity.Count : 0;
            stay.EndPaymentProcessed = true;
            //occupation.Total = (occupation.PaidRealOccupation ?? 0) + (occupation.UnpaidRealOccupation ?? 0);
        }
    }
}
