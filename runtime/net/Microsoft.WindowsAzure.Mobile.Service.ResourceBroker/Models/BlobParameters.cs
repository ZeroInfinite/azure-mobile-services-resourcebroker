using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models
{
    /// <summary>
    /// Default parameters for BLOB token generation.
    /// </summary>
    public class BlobParameters : ResourceParameters
    {
        /// <summary>
        /// Gets or sets the container name.
        /// </summary>
        public string Container
        {
            get;
            set;
        }
    }
}
