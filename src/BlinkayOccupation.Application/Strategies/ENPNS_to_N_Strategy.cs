using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;
using BlinkayOccupation.Domain.Repositories.Capacity;
using BlinkayOccupation.Domain.Repositories.Occupation;

namespace BlinkayOccupation.Application.Strategies
{
    public class ENPNS_to_N_Strategy : BaseOccupationStrategy
    {
        public ENPNS_to_N_Strategy(
            IOccupationRepository occupationRepository,
            ICapacitiesRepository capacitiesRepository)
        : base(occupationRepository, capacitiesRepository) { }

        protected override void ApplyOccupationChanges(Stays stay, Occupations occupation, Capacities? capacity, Occupations? oldOccupation = null)
        {
            occupation.UnpaidRealOccupation += 1;
            occupation.Total = capacity != null ? capacity.Count : 0;
        }
    }
}
