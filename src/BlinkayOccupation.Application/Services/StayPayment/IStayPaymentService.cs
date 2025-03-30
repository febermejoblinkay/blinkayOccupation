using BlinkayOccupation.Domain.Models;

namespace BlinkayOccupation.Application.Services.StayPayment
{
    public interface IStayPaymentService
    {
        Task ProcessInitEndPaymentStay();
        Task ProcessOccupationsSnapshot();
        Task<List<Installations>> GetAllInstallationsAsync();
        Task CloneOccupationForInstallation(Installations installation);
    }
}
