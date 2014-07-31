using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models;

namespace Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Brokers
{
    /// <summary>
    /// Generates tokens or connection strings for an Azure resource.
    /// </summary>
    public abstract class AzureResourceBroker
    {
        /// <summary>
        /// Initializes a new instance of the AzureResourceBroker class.
        /// </summary>
        /// <param name="parameters">The optional parameters.</param>
        public AzureResourceBroker(ResourceParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            this.Parameters = parameters;

        }

        /// <summary>
        /// Gets the optional parameters.
        /// </summary>

        public ResourceParameters Parameters
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a resource specific broker instance.
        /// </summary>
        /// <param name="type">The resource type.</param>
        /// <param name="connectionString">Optional connection string for the resource.</param>
        /// <param name="parameters">The resource parameters.</param>
        /// <returns>Returns the broker.</returns>
        public static AzureResourceBroker Create(ResourceType type, string connectionString, ResourceParameters parameters)
        {
            switch (type)
            {
                case ResourceType.Blob:
                    return new AzureBlobBroker(connectionString, parameters);
                case ResourceType.Table:
                    return new AzureTableBroker(connectionString, parameters);
                default:
                    throw new ArgumentException("type");
            }
        }

        /// <summary>
        /// Generates the resource token.
        /// </summary>
        /// <returns>Returns the resource or null.</returns>
        public abstract ResourceResponseToken CreateResourceToken();
    }
}