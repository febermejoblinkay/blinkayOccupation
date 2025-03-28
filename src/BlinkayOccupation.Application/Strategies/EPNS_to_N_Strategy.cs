using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;
using BlinkayOccupation.Domain.Repositories.Capacity;
using BlinkayOccupation.Domain.Repositories.Occupation;

namespace BlinkayOccupation.Application.Strategies
{
    public class EPNS_to_N_Strategy : BaseOccupationStrategy
    {
        public EPNS_to_N_Strategy(
            IOccupationRepository occupationRepository,
            ICapacitiesRepository capacitiesRepository)
        : base(occupationRepository, capacitiesRepository) { }

        protected override void ApplyOccupationChanges(Stays stay, Occupations occupation, Capacities? capacity, Occupations? oldOccupation = null)
        {
            if (stay.InitPaymentDate.HasValue && stay.Installation.DateTimeNow() > stay.InitPaymentDate.Value)
            {
                stay.InitPaymentProcessed = true;
            }

            occupation.PaidOccupation += 1;
            occupation.PaidRealOccupation += 1;
            occupation.Total = capacity != null ? capacity.Count : 0;
            //occupation.Total = (occupation.PaidRealOccupation ?? 0) + (occupation.PaidOccupation ?? 0);
        }
    }
}
