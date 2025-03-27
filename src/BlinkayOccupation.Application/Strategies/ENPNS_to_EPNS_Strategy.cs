using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;
using BlinkayOccupation.Domain.Repositories.Capacity;
using BlinkayOccupation.Domain.Repositories.Occupation;

namespace BlinkayOccupation.Application.Strategies
{
    public class ENPNS_to_EPNS_Strategy : BaseOccupationStrategy
    {
        public ENPNS_to_EPNS_Strategy(IOccupationRepository occupationRepository, ICapacitiesRepository capacitiesRepository)
            : base(occupationRepository, capacitiesRepository) { }

        protected override void ApplyOccupationChanges(Occupations occupation, Capacities? capacity, DateTime? paymentEndDate = null, Occupations? oldOccupation = null)
        {
            if (oldOccupation != null)
            {
                oldOccupation.UnpaidRealOccupation = (oldOccupation.UnpaidRealOccupation ?? 0) - 1;
            }

            occupation.PaidRealOccupation = (occupation.PaidRealOccupation ?? 0) + 1;
            occupation.PaidOccupation = (occupation.PaidOccupation ?? 0) + 1;
            occupation.Total = capacity != null ? capacity.Count : 0;

            //occupation.Total = (occupation.PaidRealOccupation ?? 0) + (occupation.UnpaidRealOccupation ?? 0);
        }
    }
}
