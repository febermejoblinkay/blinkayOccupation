namespace BlinkayOccupation.Application.Models
{
    public enum CameraOperationCase
    {
        Undefined = 0,
        PayWithFullMatch = 1,
        PayWithOnlyEntryMatch = 2,
        PayWithOnlyExitMatch = 3,
        PayWithNoMatch = 4,
        OrphanEntry = 5,
        OrphanExit = 6,
        EntryAndExitMatchWithNoPay = 7,
        DontExist = 8
    }
}
