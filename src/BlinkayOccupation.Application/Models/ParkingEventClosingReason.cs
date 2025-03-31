namespace BlinkayOccupation.Application.Models
{
    public enum ParkingEventClosingReason
    {
        Undefined,
        Exit,
        MaxParkingEventDurationReached,
        DuplicatedEnter,
        PlateUpdated,
        NoMatchingVehicle
    }
}
