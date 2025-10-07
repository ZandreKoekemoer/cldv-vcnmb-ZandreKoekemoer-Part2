using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

//Reference: Reece Waving. 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 2: Adding Image Uploads with Blob Storage.
// According to Reece Waving (2025), image files can be uploaded to Azure Blob Storage by opening a file stream and sending it via the BlobService client.
// I used this reference to understand how to implement Blob operations in my project.
namespace MVCRetailStore.Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public BlobService(string connectionString, string containerName)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerName = containerName;
            var container = _blobServiceClient.GetBlobContainerClient(_containerName);
            container.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<string> UploadsAsync(Stream fileStream, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = GetContentType(fileName) }
            };

            await blobClient.UploadAsync(fileStream, options);
            return blobClient.Uri.ToString();
        }

        public async Task DeleteBlobAsync(string blobUri)
        {
            var uri = new Uri(blobUri);
            string blobName = Uri.UnescapeDataString(uri.Segments[^1]);
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }

        private static string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}

/*
Reece Waving. 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 2: Adding Image Uploads with Blob Storage! (Version 2.0) [Source code].
Available at: <https://youtu.be/CuszKqZvRuM?si=lTfJaqI02wmHcIkh>
[Accessed 26 August 2025].
*/