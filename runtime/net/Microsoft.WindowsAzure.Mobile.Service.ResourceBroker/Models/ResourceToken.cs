using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models
{
    public class ResourceToken
    {
        /// <summary>
        /// Gets or sets the resource URI.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri
        {
            get;
            set;
        }
    }
}
