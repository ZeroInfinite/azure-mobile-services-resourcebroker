using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models
{
    /// <summary>
    /// The possible resource access permissions.
    /// </summary>
    public enum ResourcePermissions
    {
        /// <summary>
        /// Read access only.
        /// </summary>
        None = 0,

        /// <summary>
        /// Full read access.
        /// </summary>
        Read = 1,

        /// <summary>
        /// Access to add an item.
        /// </summary>
        Add = 2,

        /// <summary>
        /// Access to update an item.
        /// </summary>
        Update = 4,

        /// <summary>
        /// Access to delete an item.
        /// </summary>
        Delete = 8,

        /// <summary>
        /// Full write access.
        /// </summary>
        Write = Add | Update | Delete,

        /// <summary>
        /// Full read and write access.
        /// </summary>
        ReadWrite = Read | Write
    }
}