using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;

namespace BlinkayOccupation.Domain.Repositories.Attachment
{
    public class AttachmentRepository : IAttachmentRepository
    {
        public async Task AddAsync(Attachments attachment, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (attachment == null) throw new ArgumentException("attachment Object can not be null.", nameof(attachment));

            await context.Attachments.AddAsync(attachment);
            await context.SaveChangesAsync();
        }
    }
}
