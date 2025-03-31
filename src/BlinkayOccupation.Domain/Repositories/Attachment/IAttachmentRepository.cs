using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;

namespace BlinkayOccupation.Domain.Repositories.Attachment
{
    public interface IAttachmentRepository
    {
        Task AddAsync(Attachments attachment, BControlDbContext context);
    }
}
