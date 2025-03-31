using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;

namespace BlinkayOccupation.Domain.Repositories.ParkingEvent
{
    public interface IParkingEventsRepository
    {
        Task<Models.ParkingEvents?> GetByIdAsync(string id, BControlDbContext context);
        Task<List<Models.ParkingEvents?>> GetByIdsAsync(List<string> ids, BControlDbContext context);
        Task UpdateAsync(ParkingEvents pEvent, BControlDbContext context);
        Task<List<ParkingEvents>> ListOpenByPlate(OpenFrom openFrom, IEnumerable<string> plates, BControlDbContext context);
        Task AddAsync(ParkingEvents pEvent, BControlDbContext context);

        public readonly record struct OpenFrom(Installations Installation, DateTime Now)
        {
            public DateTime From
            {
                get { return Now - Installation.ConfigurationEventMatchingSpan; }
            }

            public TimeSpan MaxParkingEventDuration
            {
                get { return Installation.ConfigurationMaxParkingEventDuration; }
            }
        }
    }
}
