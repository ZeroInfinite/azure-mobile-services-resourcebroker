using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models;
using Newtonsoft.Json.Linq;

namespace Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Brokers
{
    /// <summary>
    /// Generates tokens or connection strings for an Azure resource.
    /// </summary>
    public abstract class AzureResourceBroker
    {
        /// <summary>
        /// Gets the collection of allowed permissions for this resource type.
        /// </summary>
        public abstract ResourcePermissions AllowedPermissions
        {
            get;
        }

        /// <summary>
        /// Creates a resource specific broker instance.
        /// </summary>
        /// <param name="resourceKey">The resource type key.</param>
        /// <returns>Returns the broker.</returns>
        public static AzureResourceBroker Create(string resourceKey)
        {
            switch (resourceKey)
            {
                case AzureBlobBroker.ResourceKey:
                    return new AzureBlobBroker();
                case AzureTableBroker.ResourceKey:
                    return new AzureTableBroker();
                case AzureQueueBroker.ResourceKey:
                    return new AzureQueueBroker();
                default:
                    throw new ArgumentException("resourceKey");
            }
        }

        /// <summary>
        /// Generates the resource token.
        /// </summary>
        /// <param name="connectionString">Optional connection string for the resource.</param>
        /// <param name="parameters">The resource parameters.</param>
        /// <returns>Returns the resource or null.</returns>
        public abstract ResourceToken CreateResourceToken(string connectionString, ResourceParameters parameters);

        /// <summary>
        /// Extracts the parameters for a request.
        /// </summary>
        /// <param name="parameters">The parameters to extract.</param>
        /// <param name="defaultParameters">Optional parameters instance to use.</param>
        /// <returns>Returns the extracted parameters.</returns>
        public virtual ResourceParameters ExtractParameters(JToken parameters, ResourceParameters defaultParameters = null)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

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
                defaultParameters.Permissions = this.ExtractPermissions(permissions);

                // Expiration.
                defaultParameters.Expiration = (DateTime)parameters["expiry"];
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            return defaultParameters;
        }

        /// <summary>
        /// Extracts the connection string for the resource from the given settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>The connection string.</returns>
        public abstract string ExtractConnectionString(IDictionary<string, string> settings);

        /// <summary>
        /// Extracts the requested permission string.
        /// </summary>
        /// <param name="permissions">The permissions to extract.</param>
        /// <returns>The extracted permissions.</returns>
        public virtual ResourcePermissions ExtractPermissions(string permissions)
        {
            if (string.IsNullOrWhiteSpace(permissions))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            ResourcePermissions allowed = this.AllowedPermissions;

            ResourcePermissions p = ResourcePermissions.None;

            foreach (char c in permissions)
            {
                switch (c)
                {
                    case 'r':
                        this.ValidatePermission(ResourcePermissions.Read, p, allowed);
                        p |= ResourcePermissions.Read;
                        break;

                    case 'a':
                        this.ValidatePermission(ResourcePermissions.Add, p, allowed);
                        p |= ResourcePermissions.Add;
                        break;

                    case 'u':
                        this.ValidatePermission(ResourcePermissions.Update, p, allowed);
                        p |= ResourcePermissions.Update;
                        break;

                    case 'd':
                        this.ValidatePermission(ResourcePermissions.Delete, p, allowed);
                        p |= ResourcePermissions.Delete;
                        break;

                    case 'p':
                        this.ValidatePermission(ResourcePermissions.Process, p, allowed);
                        p |= ResourcePermissions.Process;
                        break;

                    case 'w':
                        this.ValidatePermission(ResourcePermissions.Write, p, allowed);
                        p |= ResourcePermissions.Write;
                        break;

                    default:
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                }
            }

            return p;
        }

        /// <summary>
        /// Throws a bad request exception if the given permission set already contains the specific permission.
        /// </summary>
        /// <param name="existingPermissions">The current permission set.</param>
        /// <param name="newPermission">The specific permission to check for.</param>
        protected virtual void ValidatePermission(ResourcePermissions newPermission, ResourcePermissions existingPermissions, ResourcePermissions allowedPermissions)
        {
            // First, determine whether we've already seen this permission before.
            if ((existingPermissions & newPermission) == newPermission)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            // Now, verify that this permission is allowed.
            if ((allowedPermissions & newPermission) != newPermission)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }
    }
}