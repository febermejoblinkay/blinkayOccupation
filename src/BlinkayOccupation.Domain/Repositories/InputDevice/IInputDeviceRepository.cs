using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;

namespace BlinkayOccupation.Domain.Repositories.InputDevice
{
    public interface IInputDeviceRepository
    {
        Task UpdateAsync(InputDevices device, BControlDbContext context);
        Task<InputDevices?> GetByIdAsync(string id, BControlDbContext context);
    }
}
