using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlinkayOccupation.Application.Models;
using Microsoft.Extensions.Logging;

namespace BlinkayOccupation.Application.Services.AzureBlob
{
    public class AzureBlobStorage : IBlobStorage
    {
        private ChainedTokenCredential _tokenCredential = null!;
        private BlobServiceClient _blobServiceClient;
        private BlobContainerClient _containerClient;
        private readonly ILogger<AzureBlobStorage> _logger;

        public AzureBlobStorage(ILogger<AzureBlobStorage> logger)
        {
            _tokenCredential = new ChainedTokenCredential(new DefaultAzureCredential(), new ManagedIdentityCredential(), new AzureCliCredential());
            _logger = logger;
        }

        public void SetEndpoint(string blobEndpoint)
        {
            _blobServiceClient = new BlobServiceClient(new Uri(blobEndpoint));
        }

        public bool SetContainer(string containerName)
        {
            _logger.LogInformation("Setting container {ContainerName}", containerName);

            if (_blobServiceClient is null)
            {
                _logger.LogWarning("Endpoint not set.");
                return false;
            }

            try
            {
                _containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                if (_containerClient.Exists())
                {
                    return true;
                }
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, "Error setting container");
            }
            return false;
        }

        public async Task SaveAsync(string id, Blob blob)
        {
            if (_containerClient == null)
            {
                _logger.LogWarning("No container client.");
                throw new InvalidOperationException("Container client not set.");
            }

            var client = _containerClient.GetBlobClient(id);

            _logger.LogInformation("Uploading blob {BlobName} to container {ContainerName}", id, _containerClient.Name);

            await client.UploadAsync(blob.Stream, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = blob.ContentType },
                Metadata = GetMetadata(blob.Metadata),
                AccessTier = AccessTier.Cold
            });
        }

        public async Task SaveAsync(IEnumerable<Blob> blobs)
        {
            if (_containerClient == null)
            {
                _logger.LogWarning("No container client.");
                throw new InvalidOperationException("Container client not set.");
            }

            var tasks = blobs.Select(async blob =>
            {
                var client = _containerClient.GetBlobClient(Guid.NewGuid().ToString());

                _logger.LogInformation("Uploading blob {BlobName} to container {ContainerName}", client.Name, _containerClient.Name);

                await client.UploadAsync(blob.Stream, new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders { ContentType = blob.ContentType },
                    Metadata = GetMetadata(blob.Metadata),
                    AccessTier = AccessTier.Cold
                });
            });

            await Task.WhenAll(tasks);
        }

        private static Dictionary<string, string>? GetMetadata(IReadOnlyDictionary<string, string>? blobMetadata)
        {
            return blobMetadata?.ToDictionary(k => k.Key, v => v.Value);
        }

        public string GetBlobEndpoint()
        {
            return _containerClient?.Uri.AbsoluteUri ?? string.Empty;
        }
    }
}
