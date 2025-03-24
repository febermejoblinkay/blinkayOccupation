using BlinkayOccupation.Application.Models;

namespace BlinkayOccupation.Application.Services.Stay
{
    public interface IStayService
    {
        Task<string> AddStay(AddStayRequest request);
        Task<string> UpdateStay(UpdateStayRequest request);
    }
}
