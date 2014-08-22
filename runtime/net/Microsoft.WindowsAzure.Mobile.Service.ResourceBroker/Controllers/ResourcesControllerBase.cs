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

namespace Microsoft.WindowsAzure.Mobile.Service.ResourceBroker
{
    /// <summary>
    /// Issues tokens and connection strings for various Azure resources.
    /// </summary>
    [AuthorizeLevel(AuthorizationLevel.User)]
    public abstract class ResourcesControllerBase : ApiController
    {
        private ResourceRequestManager requestManager = new ResourceRequestManager();

        /// <summary>
        /// The services property.
        /// </summary>
        public ApiServices Services
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the request manager helper instance.
        /// </summary>
        protected ResourceRequestManager RequestManager
        {
            get
            {
                return this.requestManager;
            }
        }

        /// <summary>
        /// A generic Get method.
        /// </summary>
        public void Get()
        {
        }

        /// <summary>
        /// Generates a token or connection string based on the given configuration.
        /// </summary>
        /// <param name="type">The type of the resource to generate the token for.</param>
        /// <param name="parameters">Token parameters.</param>
        /// <returns>Returns the generated SAS token or connection string.</returns>
        public ResourceToken Post(string type, JToken parameters)
        {
            return this.GenerateToken(type, parameters);
        }

        /// <summary>
        /// Generates a resource token.
        /// </summary>
        /// <param name="type">The type of the resource to generate the token for.</param>
        /// <param name="parameters">Token parameters.</param>
        /// <returns>Returns the generated SAS token or connection string.</returns>
        protected virtual ResourceToken GenerateToken(string type, JToken parameters)
        {
            return this.requestManager.GenerateToken(type, parameters, this.Services.Settings);
        }
    }
}