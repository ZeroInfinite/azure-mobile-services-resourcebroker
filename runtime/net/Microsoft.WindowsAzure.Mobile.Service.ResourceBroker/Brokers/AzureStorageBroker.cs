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
    public abstract class AzureStorageBroker : AzureResourceBroker
    {
        /// <summary>
        /// Initializes a new instance of the AzureStorageBroker class.
        /// </summary>
        public AzureStorageBroker()
        {
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
            settings.TryGetValue("ResourceBrokerStorageConnectionString", out connectionString);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("The connection string could not be loaded.");
            }

            return connectionString;
        }
    }
}