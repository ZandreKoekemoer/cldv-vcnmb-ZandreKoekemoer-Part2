using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using MVCRetailStore.Models;

//Reference: Reece Waving. 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 4: Mastering Azure File Share
// According to Reece Waving (2025), Azure File Share operations like uploading, listing, and downloading files can be implemented using the Azure.Storage.Files.Shares SDK.
// I used this approach in AzureFileShareService to handle file uploads and downloads for the contracts file share.
namespace MVCRetailStore.Services
{
    public class AzureFileShareService
    {
        private readonly string _connectionString;
        private readonly string _fileShareName;

        public AzureFileShareService(string connectionString, string fileShareName)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _fileShareName = fileShareName ?? throw new ArgumentNullException(nameof(fileShareName));
        }

        public async Task UploadFileAsync(string directoryName, string fileName, Stream fileStream)
        {
            try
            {
                var serviceClient = new ShareServiceClient(_connectionString);
                var shareClient = serviceClient.GetShareClient(_fileShareName);
                await shareClient.CreateIfNotExistsAsync();

                var directoryClient = shareClient.GetDirectoryClient(directoryName);
                await directoryClient.CreateIfNotExistsAsync();

                var fileClient = directoryClient.GetFileClient(fileName);
                await fileClient.CreateAsync(fileStream.Length);
                await fileClient.UploadRangeAsync(new HttpRange(0, fileStream.Length), fileStream);
            }
            catch (Exception ex)
            {
                throw new Exception("Error uploading file: " + ex.Message, ex);
            }
        }

        public async Task<Stream> DownLoadFileAsync(string directoryName, string fileName)
        {
            try
            {
                var serviceClient = new ShareServiceClient(_connectionString);
                var shareClient = serviceClient.GetShareClient(_fileShareName);
                var directoryClient = shareClient.GetDirectoryClient(directoryName);
                var fileClient = directoryClient.GetFileClient(fileName);
                var downloadInfo = await fileClient.DownloadAsync();
                return downloadInfo.Value.Content;
            }
            catch (Exception ex)
            {
                throw new Exception("Error downloading file: " + ex.Message, ex);
            }
        }

        public async Task<List<FileModel>> ListFilesAsync(string directoryName)
        {
            var fileModels = new List<FileModel>();
            try
            {
                var serviceClient = new ShareServiceClient(_connectionString);
                var shareClient = serviceClient.GetShareClient(_fileShareName);
                await shareClient.CreateIfNotExistsAsync();

                var directoryClient = shareClient.GetDirectoryClient(directoryName);
                await directoryClient.CreateIfNotExistsAsync();

                await foreach (ShareFileItem item in directoryClient.GetFilesAndDirectoriesAsync())
                {
                    if (!item.IsDirectory)
                    {
                        var fileClient = directoryClient.GetFileClient(item.Name);
                        var properties = await fileClient.GetPropertiesAsync();
                        fileModels.Add(new FileModel
                        {
                            Name = item.Name,
                            Size = properties.Value.ContentLength,
                            LastModified = properties.Value.LastModified
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error listing files: " + ex.Message, ex);
            }
            return fileModels;
        }
    }
}


/*
Reece Waving. 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 4: Mastering Azure File Share! (Version 2.0) [Source code].
Available at: <https://youtu.be/A-mVVL88oEg?si=sUYFyrY2wQc6Lny0>
[Accessed 26 August 2025].
*/