using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;
using BlinkayOccupation.Domain.Repositories.Capacity;
using BlinkayOccupation.Domain.Repositories.Occupation;

namespace BlinkayOccupation.Application.Strategies
{
    public class ENPNS_to_EPS_Strategy : BaseOccupationStrategy
    {
        public ENPNS_to_EPS_Strategy(
            IOccupationRepository occupationRepository,
            ICapacitiesRepository capacitiesRepository)
            : base(occupationRepository, capacitiesRepository) { }

        protected override void ApplyOccupationChanges(Stays stay, Occupations occupation, Capacities? capacity, Occupations? oldOccupation = null)
        {
            if (oldOccupation != null)
            {
                oldOccupation.UnpaidRealOccupation = (oldOccupation.UnpaidRealOccupation ?? 0) > 0 ? oldOccupation.UnpaidRealOccupation - 1 : 0;
            }

            if (stay.EndPaymentDate.HasValue && stay.EndPaymentDate.Value > stay.Installation.DateTimeNow())
            {
                occupation.PaidOccupation += 1;
                //occupation.Total = (occupation.PaidRealOccupation ?? 0) + (occupation.UnpaidRealOccupation ?? 0);
            }

            stay.EndPaymentProcessed = true;
            occupation.Total = capacity != null ? capacity.Count : 0;
            //occupation.Total = (occupation.PaidRealOccupation ?? 0) + (occupation.UnpaidRealOccupation ?? 0);
        }
    }
}
