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
        private const string DefaultBrokerApiName = "resources";

        /// <summary>
        /// The <see cref="ResourceBrokerStorageClient"/> which is used to talk to the Azure
        /// Storage service.
        /// </summary>
        public static ResourceBrokerStorageClient BrokerStorageClient { get; set; }

        static ResourceBrokerClientExtensions()
        {
            BrokerStorageClient = new ResourceBrokerStorageClient();
        }

        /// <summary>
        /// Uploads a file to blob storage via a mobile service with the resource broker configured in
        /// as an API with the default name ("resources").
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
        public static Task<Uri> UploadFileToBlobStorage(this IMobileServiceClient client, string containerName, string fileName, string mediaType, Stream fileContents)
        {
            return UploadFileToBlobStorage(client, DefaultBrokerApiName, containerName, fileName, mediaType, fileContents);
        }

        /// <summary>
        /// Uploads a file to blob storage via a mobile service with the resource broker configured.
        /// </summary>
        /// <remarks>
        /// For more information on the resource broker, see its 
        /// <a href="https://github.com/azure/azure-mobile-services-resourcebroker">GitHub page</a>. The
        /// broker should be available in the "/api/resources" address.
        /// </remarks>
        /// <param name="client">The client for the mobile service with the resource broker configured.</param>
        /// <param name="brokerApiName">The name of the API where the resource broker lives in the service.</param>
        /// <param name="containerName">The name of the container where the blob should be uploaded.</param>
        /// <param name="fileName">The name of the blob.</param>
        /// <param name="contentType">The type of data to be stored in the blob.</param>
        /// <param name="fileContents">The contents of the blob.</param>
        /// <returns>The URL in the blob storage where the file is stored.</returns>
        public static async Task<Uri> UploadFileToBlobStorage(this IMobileServiceClient client, string brokerApiName, string containerName, string fileName, string contentType, Stream fileContents)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            if (string.IsNullOrEmpty(brokerApiName))
            {
                throw new ArgumentNullException("brokerApiName");
            }

            if (string.IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException("containerName");
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            if (string.IsNullOrEmpty(contentType))
            {
                throw new ArgumentNullException("contentType");
            }

            if (fileContents == null)
            {
                throw new ArgumentNullException("fileContents");
            }

            string sasTokenUri = await GetBlobSasTokenFromResourceBroker(client, brokerApiName, containerName, fileName);
            return await BrokerStorageClient.UploadContentToBlobStorage(containerName, fileName, contentType, fileContents, sasTokenUri);
        }

        private static async Task<string> GetBlobSasTokenFromResourceBroker(IMobileServiceClient client, string brokerApiName, string containerName, string fileName)
        {
            var sasRequestPayload = new JObject();
            sasRequestPayload.Add("name", fileName);
            sasRequestPayload.Add("container", containerName);
            sasRequestPayload.Add("permissions", "w");
            sasRequestPayload.Add("expiry", DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture));
            string sasTokenUri = null;

            try
            {
                var sasToken = await client.InvokeApiAsync(brokerApiName + "/blob", sasRequestPayload);
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
    }
}
