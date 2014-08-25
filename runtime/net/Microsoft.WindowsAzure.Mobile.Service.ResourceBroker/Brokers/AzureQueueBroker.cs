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
    public class AzureQueueBroker : AzureResourceBroker
    {
        private ResourceParameters queueParameters;
        private StorageProvider storageProvider;

        /// <summary>
        /// Initializes a new instance of the AzureQueueBroker class.
        /// </summary>
        /// <param name="storageConnectionString">The Azure storage connection string.</param>
        /// <param name="parameters">The optional parameters.</param>
        public AzureQueueBroker(string storageConnectionString, ResourceParameters parameters)
            : base(parameters)
        {
            if (string.IsNullOrWhiteSpace(storageConnectionString))
            {
                throw new ArgumentException("storageConnectionString is invalid");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            this.queueParameters = parameters;

            if (string.IsNullOrWhiteSpace(this.queueParameters.Name))
            {
                throw new ArgumentException("The queue name must not be null or empty", "parameters.Name");
            }

            this.storageProvider = new StorageProvider(storageConnectionString);
        }

        /// <summary>
        /// Generates the resource.
        /// </summary>
        /// <returns>Returns the resource.</returns>
        public override ResourceToken CreateResourceToken()
        {
            return this.storageProvider.CreateQueueAccessToken(this.queueParameters.Name, this.queueParameters.Permissions, this.queueParameters.Expiration);
        }
    }
}
