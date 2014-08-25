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
    [Flags]
    public enum ResourcePermissions
    {
        /// <summary>
        /// No access.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Full read access.
        /// </summary>
        Read = 0x1,

        /// <summary>
        /// Access to add an item.
        /// </summary>
        Add = 0x2,

        /// <summary>
        /// Access to update an item.
        /// </summary>
        Update = 0x4,

        /// <summary>
        /// Access to delete an item.
        /// </summary>
        Delete = 0x8,

        /// <summary>
        /// Access to process items (such as queue messages).
        /// </summary>
        Process = 0x10,

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