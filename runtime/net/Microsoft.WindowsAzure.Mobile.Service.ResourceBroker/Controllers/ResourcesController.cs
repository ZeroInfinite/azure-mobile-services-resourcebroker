using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Brokers;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Newtonsoft.Json.Linq;

namespace Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Controllers
{
    /// <summary>
    /// Issues tokens and connection strings for various Azure resources.
    /// </summary>
    [AuthorizeLevel(AuthorizationLevel.Anonymous)]
    public class ResourcesController : ApiController
    {
        /// <summary>
        /// Generates a token or connection string based on the given configuration.
        /// </summary>
        /// <param name="type">The type of the resource to generate the token for.</param>
        /// <param name="parameters">Optional token parameters.</param>
        /// <returns>Returns the generated SAS token or connection string.</returns>
        public ResourceResponseToken Post(string type, [FromBody] JToken parameters)
        {
            ResourceType resourceType = this.MapResourceType(type);
            ResourceParameters defaultParams = this.ExtractParameters(resourceType, parameters);

            string blobConnectionString = this.Services.Settings["RESOURCE_BROKER_BLOB_CONNECTION_STRING"];
            if (string.IsNullOrWhiteSpace(blobConnectionString))
            {
                throw new InvalidOperationException("The RESOURCE_BROKER_BLOB_ACCOUNT setting is missing or invalid.");
            }

            AzureResourceBroker broker = AzureResourceBroker.Create(resourceType, blobConnectionString, defaultParams);
            return broker.CreateResourceToken();
        }

        /// <summary>
        /// The services property.
        /// </summary>
        public ApiServices Services
        {
            get;
            set;
        }

        private ResourceType MapResourceType(string type)
        {
            if (string.Equals(type, "blob", StringComparison.OrdinalIgnoreCase))
            {
                return ResourceType.Blob;
            }
            else if (string.Equals(type, "table", StringComparison.OrdinalIgnoreCase))
            {
                return ResourceType.Table;
            }

            throw new HttpResponseException(HttpStatusCode.BadRequest);
        }

        private ResourceParameters ExtractParameters(ResourceType resourceType, JToken parameters)
        {
            if (resourceType == ResourceType.Blob)
            {
                return this.ExtractBlobParameters(parameters);
            }
            else
            {
                return this.ExtractDefaultParameters(parameters);
            }
        }

        private BlobParameters ExtractBlobParameters(JToken parameters)
        {
            BlobParameters blobParameters = new BlobParameters();

            this.ExtractDefaultParameters(parameters, blobParameters);

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

        private ResourceParameters ExtractDefaultParameters(JToken parameters, ResourceParameters defaultParameters = null)
        {
            if (defaultParameters == null)
            {
                defaultParameters = new ResourceParameters();
            }

            try
            {
                // Raw parameters.
                defaultParameters.Parameters = parameters;

                // Name.
                defaultParameters.Name = parameters.Value<string>("name");
                if (string.IsNullOrWhiteSpace(defaultParameters.Name))
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                // Permissions.
                string permissions = parameters.Value<string>("permissions");
                if (permissions == "r")
                {
                    defaultParameters.Permissions = ResourcePermissions.Read;
                }
                else if (permissions == "w")
                {
                    defaultParameters.Permissions = ResourcePermissions.Write;
                }
                else if (permissions == "rw")
                {
                    defaultParameters.Permissions = ResourcePermissions.ReadWrite;
                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                // Expiration.
                defaultParameters.Expiration = (DateTime)parameters["expiry"];
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            return defaultParameters;
        }
    }
}