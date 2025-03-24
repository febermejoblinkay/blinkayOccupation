namespace BlinkayOccupation.Domain.Helpers
{
    public interface IAuditable
    {
        DateTime Created { get; set; }
        DateTime Updated { get; set; }
        bool Deleted { get; set; }
    }
}
