using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models
{
    /// <summary>
    /// Default parameters for token generation.
    /// </summary>
    public class ResourceParameters
    {
        /// <summary>
        /// Gets or sets the resource name.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the requested permissions.
        /// </summary>
        public ResourcePermissions Permissions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the desired expiration time.
        /// </summary>
        public DateTime? Expiration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the raw parameters.
        /// </summary>
        public JToken Parameters
        {
            get;
            set;
        }
    }
}
