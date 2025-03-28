using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;
using BlinkayOccupation.Domain.Repositories.Capacity;
using BlinkayOccupation.Domain.Repositories.Occupation;

namespace BlinkayOccupation.Application.Strategies
{
    public class EPNS_to_EPS_Strategy : BaseOccupationStrategy
    {
        public EPNS_to_EPS_Strategy(IOccupationRepository occupationRepository,
            ICapacitiesRepository capacitiesRepository)
            : base(occupationRepository, capacitiesRepository) { }

        protected override void ApplyOccupationChanges(Stays stay, Occupations occupation, Capacities? capacity, Occupations? oldOccupation = null)
        {
            occupation.PaidRealOccupation = (occupation.PaidRealOccupation ?? 0) > 0 ? occupation.PaidRealOccupation - 1 : 0;
            occupation.Total = capacity != null ? capacity.Count : 0;
            stay.EndPaymentProcessed = true;
            //occupation.Total = (occupation.PaidRealOccupation ?? 0) + (occupation.UnpaidRealOccupation ?? 0);
        }
    }
}
