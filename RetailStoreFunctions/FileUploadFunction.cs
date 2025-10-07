using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Files.Shares;
using System.Net;

namespace RetailFunctions
{
    public class FileFunctions
    {
        private readonly ILogger _logger;
        private readonly string _connection = Environment.GetEnvironmentVariable("AzureStorage")!;
        private const string ShareName = "contracts";

        public FileFunctions(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<FileFunctions>();
        }

       
        [Function("ListFiles")]
        public async Task<HttpResponseData> ListFiles(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "files")] HttpRequestData req)
        {
            var share = new ShareClient(_connection, ShareName);
            await share.CreateIfNotExistsAsync();
            var root = share.GetRootDirectoryClient();

            var files = new List<object>();
            await foreach (var f in root.GetFilesAndDirectoriesAsync())
            {
                if (!f.IsDirectory)
                    files.Add(new { f.Name });
            }

            var resp = req.CreateResponse(HttpStatusCode.OK);
            await resp.WriteAsJsonAsync(files);
            return resp;
        }

        
        [Function("UploadFile")]
        public async Task<HttpResponseData> UploadFile(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "files/upload")] HttpRequestData req)
        {
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var fileName = query["fileName"];

            if (string.IsNullOrEmpty(fileName))
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync("File name is required.");
                return bad;
            }

            try
            {
                var share = new ShareClient(_connection, ShareName);
                await share.CreateIfNotExistsAsync();
                var root = share.GetRootDirectoryClient();
                var fileClient = root.GetFileClient(fileName);

                if (await fileClient.ExistsAsync())
                {
                    var existsResp = req.CreateResponse(HttpStatusCode.Conflict);
                    await existsResp.WriteStringAsync("File already exists.");
                    return existsResp;
                }

                using var ms = new MemoryStream();
                await req.Body.CopyToAsync(ms);
                ms.Position = 0;

                await fileClient.CreateAsync(ms.Length);
                await fileClient.UploadRangeAsync(new Azure.HttpRange(0, ms.Length), ms);

                var resp = req.CreateResponse(HttpStatusCode.OK);
                await resp.WriteStringAsync("File uploaded successfully.");
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upload error");
                var errorResp = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResp.WriteStringAsync($"Error: {ex.Message}");
                return errorResp;
            }
        }
    }
}

