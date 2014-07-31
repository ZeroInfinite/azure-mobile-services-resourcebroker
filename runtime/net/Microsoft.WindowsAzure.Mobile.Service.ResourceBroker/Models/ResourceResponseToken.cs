using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models
{
    public class ResourceResponseToken
    {
        /// <summary>
        /// Gets or sets the folder name.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri
        {
            get;
            set;
        }
    }
}
