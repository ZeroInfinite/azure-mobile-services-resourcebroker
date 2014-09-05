using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzure.Mobile.ResourceBroker.Client
{
    /// <summary>
    /// Class used to bridge the mobile service client and the Azure Storage service.
    /// </summary>
    /// <remarks>Since the </remarks>
    public class ResourceBrokerStorageClient
    {
        /// <summary>
        /// Defines how the <see cref="HttpClient"/> that is used to access the storage service
        /// will be created. Made internal for mocking in the unit test project.
        /// </summary>
        internal static Func<HttpClient> HttpClientCreator = () => new HttpClient();

        /// <summary>
        /// Uploads content to the blob storage.
        /// </summary>
        /// <remarks>
        /// This default client uses a simple HTTP PUT request to store the data in the blob
        /// storage; this works for small files (up to 64MB), but for larger files it will not work.
        /// In these cases, you'll need to create a subclass that handles sending the file
        /// in blocks. See the documentation for an example.
        /// </remarks>
        /// <param name="containerName">The name of the container where the blob will be uploaded.</param>
        /// <param name="blobName">The name of the blob to be uploaded.</param>
        /// <param name="contentType">The content type of the blob.</param>
        /// <param name="fileContents">The contents to be uploaded to the blob.</param>
        /// <param name="sasTokenUri">The SAS token URI returned from the resource broker.</param>
        /// <returns>The URI of the newly uploaded blob.</returns>
        public virtual async Task<Uri> UploadContentToBlobStorage(string containerName, string blobName, string contentType, Stream fileContents, string sasTokenUri)
        {
            var httpClient = HttpClientCreator();
            var req = new HttpRequestMessage(HttpMethod.Put, sasTokenUri);
            req.Headers.Add("x-ms-blob-type", "blockblob");
            req.Content = new StreamContent(fileContents);
            req.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
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
