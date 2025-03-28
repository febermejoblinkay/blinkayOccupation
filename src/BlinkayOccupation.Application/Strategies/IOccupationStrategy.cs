using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;

namespace BlinkayOccupation.Application.Strategies
{
    public interface IOccupationStrategy
    {
        Task ExecuteAsync(
            Stays stay,
            string? tariffId,
            string oldState,
            string newState,
            BControlDbContext context
        );
    }
}
