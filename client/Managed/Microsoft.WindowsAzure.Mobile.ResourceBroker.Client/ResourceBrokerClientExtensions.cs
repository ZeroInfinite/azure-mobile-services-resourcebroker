using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;

namespace Microsoft.WindowsAzure.Mobile.ResourceBroker.Client
{
    /// <summary>
    /// Provides extension methods to the <see cref="IMobileServiceClient"/> interface for accessing the
    /// resource broker on the server side.
    /// </summary>
    public static class ResourceBrokerClientExtensions
    {
        /// <summary>
        /// Defines how the <see cref="HttpClient"/> that is used to access the storage service
        /// will be created. Made internal for mocking in the unit test project.
        /// </summary>
        internal static Func<HttpClient> HttpClientCreator = () => new HttpClient();

        /// <summary>
        /// Uploads a file to blob storage via a mobile service with the resource broker configured.
        /// </summary>
        /// <remarks>
        /// For more information on the resource broker, see its 
        /// <a href="https://github.com/azure/azure-mobile-services-resourcebroker">GitHub page</a>. The
        /// broker should be available in the "/api/resources" address.
        /// </remarks>
        /// <param name="client">The client for the mobile service with the resource broker configured.</param>
        /// <param name="containerName">The name of the container where the blob should be uploaded.</param>
        /// <param name="fileName">The name of the blob.</param>
        /// <param name="mediaType">The type of data to be stored in the blob.</param>
        /// <param name="fileContents">The contents of the blob.</param>
        /// <returns>The URL in the blob storage where the file is stored.</returns>
        public static async Task<Uri> UploadFileToBlobStorage(this IMobileServiceClient client, string containerName, string fileName, string mediaType, Stream fileContents)
        {
            string sasTokenUri = await GetSasTokenFromResourceBroker(client, containerName, fileName);
            return await UploadContentToBlobStorage(mediaType, fileContents, sasTokenUri);
        }

        private static async Task<string> GetSasTokenFromResourceBroker(IMobileServiceClient client, string containerName, string fileName)
        {
            var sasRequestPayload = new JObject();
            sasRequestPayload.Add("name", fileName);
            sasRequestPayload.Add("container", containerName);
            sasRequestPayload.Add("permissions", "w");
            sasRequestPayload.Add("expiry", DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture));
            string sasTokenUri = null;

            try
            {
                var sasToken = await client.InvokeApiAsync("resources/blob", sasRequestPayload);
                sasTokenUri = (string)sasToken["uri"];
                if (sasTokenUri == null)
                {
                    throw new InvalidOperationException(Resources.InvalidBlobResponseFromBroker);
                }
            }
            catch (MobileServiceInvalidOperationException e)
            {
                if (e.Response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new InvalidOperationException(Resources.RequestToBrokerReturns404, e);
                }
                else
                {
                    throw;
                }
            }

            return sasTokenUri;
        }

        private static async Task<Uri> UploadContentToBlobStorage(string mediaType, Stream fileContents, string sasTokenUri)
        {
            var httpClient = HttpClientCreator();
            var req = new HttpRequestMessage(HttpMethod.Put, sasTokenUri);
            req.Headers.Add("x-ms-blob-type", "blockblob");
            req.Content = new StreamContent(fileContents);
            req.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
            var putResp = await httpClient.SendAsync(req);
            try
            {
                putResp.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                throw new InvalidOperationException(Resources.InvalidResponseFromStorage, e);
            }

            UriBuilder uriBuilder = new UriBuilder(sasTokenUri);
            uriBuilder.Query = null;
            uriBuilder.Fragment = null;
            return uriBuilder.Uri;
        }
    }
}
