using BlinkayOccupation.Application.Models;

namespace BlinkayOccupation.Application.Services.AzureBlob
{
    public interface IBlobStorage
    {
        void SetEndpoint(string blobEndpoint);
        bool SetContainer(string containerName);
        Task SaveAsync(string id, Blob blob);
        string GetBlobEndpoint();
        Task SaveAsync(IEnumerable<Blob> blobs);

        static string CreateId()
        {
            return $"{DateTime.UtcNow.Year:D4}-{DateTime.UtcNow.Month:D2}-{DateTime.UtcNow.Day:D2}/{Guid.CreateVersion7().ToString()}";
        }
    }
}
