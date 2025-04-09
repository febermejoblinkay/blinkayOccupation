using BlinkayOccupation.Application.Models;

namespace BlinkayOccupation.Application.Services.Occupation
{
    public interface IOccupationsService
    {
        Task<List<CurrentParkingDataDto>> GetCurrentOccupation();
    }
}
