namespace BlinkayOccupation.Application.Strategies
{
    public interface IOccupationStrategyFactory
    {
        IOccupationStrategy GetStrategy(string oldState, string newState);
    }
}
