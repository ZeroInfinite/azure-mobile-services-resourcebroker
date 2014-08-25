﻿using System;
using System.Collections.Generic;
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

namespace Microsoft.WindowsAzure.Mobile.Service.ResourceBroker
{
    /// <summary>
    /// Issues tokens and connection strings for various Azure resources.
    /// </summary>
    public class ResourceRequestManager
    {
        /// <summary>
        /// Generates a token or connection string based on the given configuration.
        /// </summary>
        /// <param name="resourceType">The type of the resource to generate the token for.</param>
        /// <param name="parameters">Token parameters.</param>
        /// <param name="connectionString">The fully privileged connection string to the resource.</param>
        /// <returns>Returns the generated SAS token or connection string.</returns>
        public ResourceToken GenerateToken(string resourceType, JToken parameters, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(resourceType))
            {
                throw new ArgumentException("must be a valid type string", "resourceType");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("must be a valid connection string", "connectionString");
            }

            ResourceType mappedResourceType = this.MapResourceType(resourceType);
            return this.GenerateToken(mappedResourceType, parameters, connectionString);
        }

        /// <summary>
        /// Generates a token or connection string based on the given configuration.
        /// </summary>
        /// <param name="resourceType">The type of the resource to generate the token for.</param>
        /// <param name="parameters">Token parameters.</param>
        /// <param name="settings">The app settings collection.</param>
        /// <returns>Returns the generated SAS token or connection string.</returns>
        public ResourceToken GenerateToken(string resourceType, JToken parameters, IDictionary<string, string> settings)
        {
            if (string.IsNullOrWhiteSpace(resourceType))
            {
                throw new ArgumentException("must be a valid type string", "resourceType");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            ResourceType mappedResourceType = this.MapResourceType(resourceType);
            return this.GenerateToken(mappedResourceType, parameters, this.GetConnectionString(mappedResourceType, settings));
        }

        /// <summary>
        /// Generates a token or connection string based on the given configuration.
        /// </summary>
        /// <param name="resourceType">The type of the resource to generate the token for.</param>
        /// <param name="parameters">Token parameters.</param>
        /// <param name="connectionString">The fully privileged connection string to the resource.</param>
        /// <returns>Returns the generated SAS token or connection string.</returns>
        public ResourceToken GenerateToken(ResourceType resourceType, JToken parameters, string connectionString)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("must be a valid connection string", "connectionString");
            }

            ResourceParameters defaultParams = this.ParseTokenParameters(resourceType, parameters);
            return this.GenerateToken(resourceType, defaultParams, connectionString);
        }

        /// <summary>
        /// Generates a token or connection string based on the given configuration.
        /// </summary>
        /// <param name="resourceType">The type of the resource to generate the token for.</param>
        /// <param name="parameters">Token parameters.</param>
        /// <param name="settings">The app settings collection.</param>
        /// <returns>Returns the generated SAS token or connection string.</returns>
        public ResourceToken GenerateToken(ResourceType resourceType, JToken parameters, IDictionary<string, string> settings)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            ResourceParameters defaultParams = this.ParseTokenParameters(resourceType, parameters);
            return this.GenerateToken(resourceType, defaultParams, this.GetConnectionString(resourceType, settings));
        }

        /// <summary>
        /// Generates a token or connection string based on the given configuration.
        /// </summary>
        /// <param name="resourceType">The type of the resource to generate the token for.</param>
        /// <param name="parameters">Token parameters.</param>
        /// <param name="connectionString">The fully privileged connection string to the resource.</param>
        /// <returns>Returns the generated SAS token or connection string.</returns>
        public ResourceToken GenerateToken(ResourceType resourceType, ResourceParameters parameters, string connectionString)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("must be a valid connection string", "connectionString");
            }

            AzureResourceBroker broker = AzureResourceBroker.Create(resourceType, connectionString, parameters);
            return broker.CreateResourceToken();
        }

        /// <summary>
        /// Generates a token or connection string based on the given configuration.
        /// </summary>
        /// <param name="resourceType">The type of the resource to generate the token for.</param>
        /// <param name="parameters">Token parameters.</param>
        /// <param name="appSettings">The app settings collection.</param>
        /// <returns>Returns the generated SAS token or connection string.</returns>
        public ResourceToken GenerateToken(ResourceType resourceType, ResourceParameters parameters, IDictionary<string, string> appSettings)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (appSettings == null)
            {
                throw new ArgumentNullException("services");
            }

            AzureResourceBroker broker = AzureResourceBroker.Create(resourceType, this.GetConnectionString(resourceType, appSettings), parameters);
            return broker.CreateResourceToken();
        }

        /// <summary>
        /// Gets a connection string for the given resource type, using the default app settings.
        /// </summary>
        /// <param name="resourceType">The type of the resource to get a connection string for.</param>
        /// <param name="settings">The app settings collection.</param>
        /// <returns>The connection string.</returns>
        public string GetConnectionString(string resourceType, IDictionary<string, string> settings)
        {
            if (string.IsNullOrWhiteSpace(resourceType))
            {
                throw new ArgumentException("must be a valid type string", "resourceType");
            }

            return this.GetConnectionString(this.MapResourceType(resourceType), settings);
        }

        /// <summary>
        /// Gets a connection string for the given resource type, using the default app settings.
        /// </summary>
        /// <param name="resourceType">The type of the resource to get a connection string for.</param>
        /// <param name="settings">The app settings collection.</param>
        /// <returns>The connection string.</returns>
        public string GetConnectionString(ResourceType resourceType, IDictionary<string, string> settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            switch (resourceType)
            {
                case ResourceType.Blob:
                case ResourceType.Table:
                case ResourceType.Queue:
                    return this.GetStorageConnectionString(resourceType, settings);

                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Converts the Json token parameters into a strongly-typed object.
        /// </summary>
        /// <param name="resourceType">The resource type parameter.</param>
        /// <param name="parameters">The JSON parameters.</param>
        /// <returns>Returns the strongly-typed parameters.</returns>
        public ResourceParameters ParseTokenParameters(string resourceType, JToken parameters)
        {
            if (string.IsNullOrWhiteSpace(resourceType))
            {
                throw new ArgumentNullException("resourceType");
            }

            return this.ParseTokenParameters(this.MapResourceType(resourceType), parameters);
        }

        /// <summary>
        /// Converts the Json token parameters into a strongly-typed object.
        /// </summary>
        /// <param name="resourceType">The resource type parameter.</param>
        /// <param name="parameters">The JSON parameters.</param>
        /// <returns>Returns the strongly-typed parameters.</returns>
        public ResourceParameters ParseTokenParameters(ResourceType resourceType, JToken parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (resourceType == ResourceType.Blob)
            {
                return this.ExtractBlobParameters(parameters);
            }
            else
            {
                return this.ExtractDefaultParameters(parameters);
            }
        }

        private string GetStorageConnectionString(ResourceType resourceType, IDictionary<string, string> settings)
        {
            const string GenericStorageParameterName = "ResourceBrokerStorageConnectionString";
            const string BlobStorageParameterName = "ResourceBrokerBlobConnectionString";
            const string TableStorageParameterName = "ResourceBrokerTableConnectionString";
            const string QueueStorageParameterName = "ResourceBrokerQueueConnectionString";

            string resourceSpecificParameterName = BlobStorageParameterName;

            if (resourceType == ResourceType.Table)
            {
                resourceSpecificParameterName = TableStorageParameterName;
            }
            else if (resourceType == ResourceType.Queue)
            {
                resourceSpecificParameterName = QueueStorageParameterName;
            }

            string connectionString = null;
            settings.TryGetValue(resourceSpecificParameterName, out connectionString);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                settings.TryGetValue(GenericStorageParameterName, out connectionString);
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Either the {0} or {1} app settings must be defined", resourceSpecificParameterName, GenericStorageParameterName));
            }

            return connectionString;
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
            else if (string.Equals(type, "queue", StringComparison.OrdinalIgnoreCase))
            {
                return ResourceType.Queue;
            }

            throw new HttpResponseException(HttpStatusCode.BadRequest);
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
                    defaultParameters.Permissions = ExtractPermissions(permissions);
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

        /// <summary>
        /// Extracts permissions based on the incoming permissions strings.
        /// </summary>
        /// <param name="permissions">The permissions string.</param>
        /// <returns>Returns the permissions.</returns>
        private static ResourcePermissions ExtractPermissions(string permissions)
        {
            if (string.IsNullOrWhiteSpace(permissions))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            ResourcePermissions p = ResourcePermissions.None;

            foreach (char c in permissions)
            {
                switch (c)
                {
                    case 'r':
                        p |= ResourcePermissions.Read;
                        break;

                    case 'a':
                        p |= ResourcePermissions.Add;
                        break;

                    case 'u':
                        p |= ResourcePermissions.Update;
                        break;

                    case 'd':
                        p |= ResourcePermissions.Delete;
                        break;

                    case 'p':
                        p |= ResourcePermissions.Process;
                        break;

                    default:
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                }
            }

            return p;
        }
    }
}