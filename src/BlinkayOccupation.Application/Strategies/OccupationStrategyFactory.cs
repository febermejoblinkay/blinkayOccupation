using BlinkayOccupation.Application.Services.Stay;
using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;
using BlinkayOccupation.Domain.Repositories.Capacity;
using BlinkayOccupation.Domain.Repositories.Occupation;
using Microsoft.Extensions.Logging;

namespace BlinkayOccupation.Application.Strategies
{
    public class OccupationStrategyFactory : IOccupationStrategyFactory
    {
        private readonly Dictionary<(string, string), IOccupationStrategy> _strategies;

        private readonly ILogger<OccupationStrategyFactory> _logger;

        public OccupationStrategyFactory(
            IOccupationRepository occupationRepository,
            ICapacitiesRepository capacitiesRepository,
            ILogger<OccupationStrategyFactory> logger)
        {
            _strategies = new Dictionary<(string, string), IOccupationStrategy>
            {
                { ("E,NP,NS", "N"), new ENPNS_to_N_Strategy(occupationRepository, capacitiesRepository) },
                { ("E,P,NS", "N"), new EPNS_to_N_Strategy(occupationRepository, capacitiesRepository) },
                { ("E,P,S", "N"), new ConditionalPaidOccupationStrategy(occupationRepository, capacitiesRepository) },
                { ("NE,P,S", "N"), new ConditionalPaidOccupationStrategy(occupationRepository, capacitiesRepository) },
                { ("NE,P,NS", "N"), new ConditionalPaidOccupationStrategy(occupationRepository, capacitiesRepository) },
                { ("E,NP,S", "EPS"), new ConditionalPaidOccupationStrategy(occupationRepository, capacitiesRepository) },
                { ("NE,NP,S", "EPS"), new ConditionalPaidOccupationStrategy(occupationRepository, capacitiesRepository) },
                { ("NE,NP,S", "NEPS"), new ConditionalPaidOccupationStrategy(occupationRepository, capacitiesRepository) },
                { ("E,NP,NS", "E,P,NS"), new ENPNS_to_EPNS_Strategy(occupationRepository, capacitiesRepository) },
                { ("E,NP,NS", "E,P,S"), new ENPNS_to_EPS_Strategy(occupationRepository, capacitiesRepository) },
                { ("E,NP,NS", "E,NP,S"), new ENPNS_to_ENPS_Strategy(occupationRepository, capacitiesRepository) },
                { ("E,P,NS", "E,P,S"), new EPNS_to_EPS_Strategy(occupationRepository, capacitiesRepository) },
                { ("NE,P,NS", "E,P,NS"), new NEPNS_to_EPNS_Strategy(occupationRepository, capacitiesRepository) }
            };
            _logger = logger;
        }

        public IOccupationStrategy GetStrategy(string oldState, string newState)
        {
            if (!_strategies.TryGetValue((oldState, newState), out var strategy))
            {
                var oldStaySplitted = oldState.Split(',');
                var isNewStay = newState.Equals("N");
                if (oldStaySplitted.Length == 3 && isNewStay &&
                    (oldStaySplitted.SequenceEqual(new[] { "NE", "NP", "S" }) ||
                     oldStaySplitted.SequenceEqual(new[] { "E", "NP", "S" })))
                {
                    return null;
                }
                else
                {
                    _logger.LogWarning("OccupationStrategy: Transition '{old}'-> '{new}' is impossible according to rules defined in matrix.", oldState, newState);
                    return new ImpossibleStrategy(oldState, newState);
                }
            }

            return strategy;
        }
    }

    public class ImpossibleStrategy : IOccupationStrategy
    {
        private readonly string _old;
        private readonly string _new;

        public ImpossibleStrategy(string oldState, string newState)
        {
            _old = oldState;
            _new = newState;
        }

        public Task ExecuteAsync(
            Stays stay,
            string? tariffId,
            string oldState,
            string newState,
            BControlDbContext context
        )
        {
            throw new InvalidOperationException(
                $"Transition '{_old}' -> '{_new}' is impossible according to rules defined in matrix."
            );
        }
    }
}
