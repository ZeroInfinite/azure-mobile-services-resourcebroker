using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Providers
{
    /// <summary>
    /// Provides access to Azure Storage accounts.
    /// </summary>
    public class StorageProvider
    {
        private CloudStorageAccount storageAccount;
        private CloudBlobClient blobClient;
        private CloudTableClient tableClient;

        /// <summary>
        /// Initializes a new instance of the StorageProvider class.
        /// </summary>
        /// <param name="connectionString">The storage connection string.</param>
        public StorageProvider(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Cannot be null or empty", "connectionString");
            }

            this.storageAccount = CloudStorageAccount.Parse(connectionString);
        }

        /// <summary>
        /// Creates a SAS key for a blob with the given name, permissions, and expiration.
        /// </summary>
        /// <param name="containerName">The name of the container to create the blob within.</param>
        /// <param name="blobName">The name of the blob to create.</param>
        /// <param name="permissions">The permissions to apply to the blob.</param>
        /// <param name="expiration">The expiration time for the token.</param>
        /// <returns>Returns the blob's URI access string.</returns>
        public ResourceResponseToken CreateBlobAccessToken(string containerName, string blobName, ResourcePermissions permissions, DateTime? expiration = null)
        {
            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentException("must not be null or empty", "containerName");
            }

            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("must not be null or empty", "blobName");
            }

            // Set up the container connection.
            CloudBlobContainer container = this.BlobClient.GetContainerReference(containerName);

            // Set up the SAS constraints.
            SharedAccessBlobPolicy sasConstraints = GetBlobSasConstraints(permissions, expiration);

            // Get a reference to the blob.
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            // Get the blob SAS.
            string sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);

            // Append this to the URI.
            return new ResourceResponseToken { Uri = blob.Uri + sasBlobToken };
        }

        /// <summary>
        /// Creates a SAS key for a table with the given name, permissions, and expiration.
        /// </summary>
        /// <param name="tableName">The name of the table to create.</param>
        /// <param name="permissions">The permissions to apply to the table.</param>
        /// <param name="expiration">The expiration time for the token.</param>
        /// <returns>Returns the blob's URI access string.</returns>
        public ResourceResponseToken CreateTableAccessToken(string tableName, ResourcePermissions permissions, DateTime? expiration = null)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("must not be null or empty", "tableName");
            }

            // Set up the SAS constraints.
            SharedAccessTablePolicy sasConstraints = GetTableSasConstraints(permissions, expiration);

            // Get a reference to the table.
            CloudTable table = this.TableClient.GetTableReference(tableName);

            // Get the table SAS.
            string sasTableToken = table.GetSharedAccessSignature(sasConstraints);

            // Append this to the URI.
            return new ResourceResponseToken { Uri = table.Uri + sasTableToken };
        }

        /// <summary>
        /// Creates SAS constraints from the given parameters.
        /// </summary>
        /// <param name="permissions">The permission set.</param>
        /// <param name="expiration">The expiration time.</param>
        /// <returns>Returns the access policy.</returns>
        private static SharedAccessBlobPolicy GetBlobSasConstraints(ResourcePermissions permissions, DateTime? expiration)
        {
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();

            // Set the start time to five minutes in the past.
            sasConstraints.SharedAccessStartTime = DateTimeOffset.Now - TimeSpan.FromMinutes(5);

            // Expiration.
            if (expiration != null)
            {
                sasConstraints.SharedAccessExpiryTime = expiration.Value;
            }

            // Permissions.
            sasConstraints.Permissions = SharedAccessBlobPermissions.None;
            if ((permissions & ResourcePermissions.Read) == ResourcePermissions.Read)
            {
                sasConstraints.Permissions |= SharedAccessBlobPermissions.Read;
            }

            if ((permissions & ResourcePermissions.Write) == ResourcePermissions.Write)
            {
                sasConstraints.Permissions |= SharedAccessBlobPermissions.Write;
            }
            else
            {
                if ((permissions & ResourcePermissions.Add) == ResourcePermissions.Add ||
                    (permissions & ResourcePermissions.Update) == ResourcePermissions.Update ||
                    (permissions & ResourcePermissions.Delete) == ResourcePermissions.Delete)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Blobs do not support Add, Update, or Delete permissions. Use the Write permission instead.") });
                }
            }

            return sasConstraints;
        }

        /// <summary>
        /// Creates SAS constraints from the given parameters.
        /// </summary>
        /// <param name="permissions">The permission set.</param>
        /// <param name="expiration">The expiration time.</param>
        /// <returns>Returns the access policy.</returns>
        private static SharedAccessTablePolicy GetTableSasConstraints(ResourcePermissions permissions, DateTime? expiration)
        {
            SharedAccessTablePolicy sasConstraints = new SharedAccessTablePolicy();

            // Set the start time to five minutes in the past.
            sasConstraints.SharedAccessStartTime = DateTimeOffset.Now - TimeSpan.FromMinutes(5);

            // Expiration.
            if (expiration != null)
            {
                sasConstraints.SharedAccessExpiryTime = expiration.Value;
            }

            // Permissions.
            sasConstraints.Permissions = SharedAccessTablePermissions.None;
            if ((permissions & ResourcePermissions.Read) == ResourcePermissions.Read)
            {
                sasConstraints.Permissions |= SharedAccessTablePermissions.Query;
            }

            if ((permissions & ResourcePermissions.Add) == ResourcePermissions.Add)
            {
                sasConstraints.Permissions |= SharedAccessTablePermissions.Add;
            }

            if ((permissions & ResourcePermissions.Update) == ResourcePermissions.Update)
            {
                sasConstraints.Permissions |= SharedAccessTablePermissions.Update;
            }

            if ((permissions & ResourcePermissions.Delete) == ResourcePermissions.Delete)
            {
                sasConstraints.Permissions |= SharedAccessTablePermissions.Delete;
            }

            return sasConstraints;
        }

        /// <summary>
        /// Gets the blob client.
        /// </summary>
        private CloudBlobClient BlobClient
        {
            get
            {
                if (this.blobClient == null)
                {
                    this.blobClient = this.storageAccount.CreateCloudBlobClient();
                }

                return this.blobClient;
            }
        }

        /// <summary>
        /// Gets the table client.
        /// </summary>
        public CloudTableClient TableClient
        {
            get
            {
                if (this.tableClient == null)
                {
                    this.tableClient = this.storageAccount.CreateCloudTableClient();
                }

                return this.tableClient;
            }
        }
    }
}
