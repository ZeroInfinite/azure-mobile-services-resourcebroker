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
    /// Generates tokens or connection strings for a table resource.
    /// </summary>
    public class AzureTableBroker : AzureResourceBroker
    {
        private ResourceParameters tableParameters;
        private StorageProvider storageProvider;

        /// <summary>
        /// Initializes a new instance of the AzureTableBroker class.
        /// </summary>
        /// <param name="storageConnectionString">The Azure storage connection string.</param>
        /// <param name="parameters">The optional parameters.</param>
        public AzureTableBroker(string storageConnectionString, ResourceParameters parameters)
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

            this.tableParameters = parameters;

            if (string.IsNullOrWhiteSpace(this.tableParameters.Name))
            {
                throw new ArgumentException("The table name must not be null or empty", "parameters.Name");
            }

            this.storageProvider = new StorageProvider(storageConnectionString);
        }

        /// <summary>
        /// Generates the resource.
        /// </summary>
        /// <returns>Returns the resource.</returns>
        public override ResourceResponseToken CreateResourceToken()
        {
            return this.storageProvider.CreateTableAccessToken(this.tableParameters.Name, this.tableParameters.Permissions, this.tableParameters.Expiration);
        }
    }
}
