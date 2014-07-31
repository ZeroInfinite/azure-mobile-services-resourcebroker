using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Brokers;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Providers;
using Moq;

namespace Test.WindowsAzure.Mobile.Service.ResourceBroker
{
    [TestClass]
    public class AzureBlobBrokerTests : TestBase
    {
        private const string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=test;AccountKey=3w1OwI/N6dqvmN0Iaa0/y6zlqL81H42K/mfIbIIKeFQkNpHSNvOcnWpucvrX5rbKGm+WKEUxaOZikeTMWpXfxA==";

        [TestMethod]
        public void Blob_Create_WithNullConnectionString_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(null, new BlobParameters { Name = "blob", Container = "container" }));
        }

        [TestMethod]
        public void Blob_Create_WithEmptyConnectionString_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker("", new BlobParameters { Name = "blob", Container = "container" }));
        }

        [TestMethod]
        public void Blob_Create_WithWhitespaceConnectionString_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(" ", new BlobParameters { Name = "blob", Container = "container" }));
        }

        [TestMethod]
        public void Blob_Create_WithNullParameters_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(ConnectionString, null));
        }

        [TestMethod]
        public void Blob_Create_WithResourceParameters_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(ConnectionString, new ResourceParameters { Name = "blob" }));
        }

        [TestMethod]
        public void Blob_Create_WithNullContainerName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = null }));
        }

        [TestMethod]
        public void Blob_Create_WithEmptyContainerName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "" }));
        }

        [TestMethod]
        public void Blob_Create_WithWhitespaceContainerName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = " " }));
        }

        [TestMethod]
        public void Blob_Create_WithNullBlobName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(ConnectionString, new BlobParameters { Name = null, Container = "container" }));
        }

        [TestMethod]
        public void Blob_Create_WithEmptyBlobName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "", Container = "container" }));
        }

        [TestMethod]
        public void Blob_Create_WithWhitespaceBlobName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(ConnectionString, new BlobParameters { Name = " ", Container = "container" }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_ReturnsCorrectHostPath()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act.
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);

            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("https://test.blob.core.windows.net/container/blob", parts.HostName);
        }

        [TestMethod]
        public void Blob_CreateResourceAsync_WithExpirationDate_ReturnsCorrectExpirationDate()
        {
            // Setup
            DateTime expiration = new DateTime(2199, 3, 12);
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read, Expiration = expiration });

            // Act.
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);

            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("2199-03-12T07%3A00%3A00Z", parts.Value("se"));
        }

        [TestMethod]
        public void Blob_CreateResourceAsync_WithNoExpirationDate_ReturnsNoExpirationDate()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read });

            // Act.
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);

            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual(null, parts.Value("se"));
        }

        [TestMethod]
        public void Blob_CreateResourceAsync_ReturnsExpectedStartDate()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read });

            // Act.
            ResourceResponseToken token = broker.CreateResourceToken();


            // Assert.
            Assert.IsNotNull(token);

            // Calculate the expected start time give or take 4 seconds.
            DateTime startRangeBegin = DateTime.UtcNow - TimeSpan.FromMinutes(5) - TimeSpan.FromSeconds(2);
            DateTime startRangeEnd = startRangeBegin + TimeSpan.FromSeconds(2);

            // Now convert these into strings using the SAS format.
            string startRangeBeginString = this.DateTimeToSASDateString(startRangeBegin);
            string startRangeEndString = this.DateTimeToSASDateString(startRangeEnd);

            // Get the actual begin time from the SAS token.
            SASParts parts = new SASParts(token.Uri);
            string beginning = parts.Value("st");

            // Make sure it is within the range.
            Assert.IsTrue(string.CompareOrdinal(beginning, startRangeBeginString) >= 0 && string.CompareOrdinal(beginning, startRangeEndString) <= 0);
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithReadPermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("r", parts.Value("sp"));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithWritePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Write, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("w", parts.Value("sp"));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithReadWritePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.ReadWrite, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rw", parts.Value("sp"));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithReadAndWritePermissions_ReturnsCorrectAccess()
        {
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Write, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rw", parts.Value("sp"));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithReadAddUpdateDeletePermissions_ReturnsCorrectAccess()
        {
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Add | ResourcePermissions.Update | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rw", parts.Value("sp"));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithAddUpdateDeletePermissions_ReturnsCorrectAccess()
        {
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Add | ResourcePermissions.Update | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("w", parts.Value("sp"));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithAddPermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Add, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithReadAddPermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Add, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithUpdatePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Update, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithReadUpdatePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Update, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithDeletePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithReadDeletePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithAddUpdatePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Add | ResourcePermissions.Update, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithReadAddUpdatePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Add | ResourcePermissions.Update, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithAddDeletePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Add | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithReadAddDeletePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Add | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithUpdateDeletePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Update | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithReadUpdateDeletePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Update | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

    }
}