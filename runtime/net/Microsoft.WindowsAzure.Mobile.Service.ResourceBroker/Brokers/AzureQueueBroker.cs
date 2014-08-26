using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Providers;

namespace Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Brokers
{
    /// <summary>
    /// Generates tokens or connection strings for a queue resource.
    /// </summary>
    public class AzureQueueBroker : AzureStorageBroker
    {
        public const string ResourceKey = "queue";

        /// <summary>
        /// Gets the collection of allowed permissions for this resource type.
        /// </summary>
        public override ResourcePermissions AllowedPermissions
        {
            get
            {
                return ResourcePermissions.Read | ResourcePermissions.Add | ResourcePermissions.Update | ResourcePermissions.Process;
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

            ResourceParameters queueParameters = parameters;

            if (string.IsNullOrWhiteSpace(queueParameters.Name))
            {
                throw new ArgumentException("The queue name must not be null or empty", "parameters.Name");
            }

            StorageProvider storageProvider = new StorageProvider(connectionString);

            return storageProvider.CreateQueueAccessToken(queueParameters.Name, queueParameters.Permissions, queueParameters.Expiration);
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
            settings.TryGetValue("ResourceBrokerQueueConnectionString", out connectionString);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return base.ExtractConnectionString(settings);
            }

            return connectionString;
        }
    }
}
