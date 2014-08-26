using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Providers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;

namespace Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Brokers
{
    /// <summary>
    /// Generates tokens or connection strings for a BLOB resource.
    /// </summary>
    public class AzureBlobBroker : AzureStorageBroker
    {
        public const string ResourceKey = "blob";

        /// <summary>
        /// Gets the collection of allowed permissions for this resource type.
        /// </summary>
        public override ResourcePermissions AllowedPermissions
        {
            get
            {
                return ResourcePermissions.Read | ResourcePermissions.Write;
            }
        }

        /// <summary>
        /// Generates the resource.
        /// </summary>
        /// <param name="connectionString">Optional connection string for the resource.</param>
        /// <param name="parameters">The resource parameters.</param>
        /// <returns>Returns the resource.</returns>
        public override ResourceToken CreateResourceToken(string connectionString, ResourceParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("connectionString is invalid");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            BlobParameters blobParameters = parameters as BlobParameters;

            if (blobParameters == null)
            {
                throw new ArgumentException("Expected a BlobParameters collection", "parameters");
            }

            if (string.IsNullOrWhiteSpace(blobParameters.Container))
            {
                throw new ArgumentException("The container name must not be null or empty", "parameters.Container");
            }

            if (string.IsNullOrWhiteSpace(blobParameters.Name))
            {
                throw new ArgumentException("The blob name must not be null or empty", "parameters.Name");
            }

            StorageProvider storageProvider = new StorageProvider(connectionString);

            return storageProvider.CreateBlobAccessToken(blobParameters.Container, blobParameters.Name, blobParameters.Permissions, blobParameters.Expiration);
        }

        /// <summary>
        /// Extracts the connection string for the resource from the given settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>The connection string.</returns>
        public override string ExtractConnectionString(IDictionary<string, string> settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            string connectionString = null;
            settings.TryGetValue("ResourceBrokerBlobConnectionString", out connectionString);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return base.ExtractConnectionString(settings);
            }

            return connectionString;
        }

        /// <summary>
        /// Extracts the parameters for a request.
        /// </summary>
        /// <param name="parameters">The parameters to extract.</param>
        /// <param name="defaultParameters">Optional parameters instance to use.</param>
        /// <returns>Returns the extracted parameters.</returns>
        public override ResourceParameters ExtractParameters(JToken parameters, ResourceParameters defaultParameters = null)
        {
            BlobParameters blobParameters = null;

            if (defaultParameters == null)
            {
                blobParameters = new BlobParameters();
            }
            else
            {
                blobParameters = defaultParameters as BlobParameters;
                if (blobParameters == null)
                {
                    throw new ArgumentException("Must be a BlobParameters instance", "defaultParameters");
                }
            }

            base.ExtractParameters(parameters, blobParameters);

            try
            {
                // Container.
                blobParameters.Container = parameters.Value<string>("container");
                if (string.IsNullOrWhiteSpace(blobParameters.Container))
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            return blobParameters;
        }
    }
}