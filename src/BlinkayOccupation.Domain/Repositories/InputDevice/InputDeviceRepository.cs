using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BlinkayOccupation.Domain.Repositories.InputDevice
{
    public class InputDeviceRepository : IInputDeviceRepository
    {
        public async Task UpdateAsync(InputDevices device, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (device == null) throw new ArgumentException("Stay Object can not be null.", nameof(device));

            context.InputDevices.Update(device);
            await context.SaveChangesAsync();
        }

        public async Task<InputDevices?> GetByIdAsync(string id, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Id cannot be null.", nameof(id));

            return await context.InputDevices
                .Include(s => s.Installation)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
