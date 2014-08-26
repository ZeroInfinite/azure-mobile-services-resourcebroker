using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Brokers;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Providers;
using Moq;
using Newtonsoft.Json.Linq;

namespace Test.WindowsAzure.Mobile.Service.ResourceBroker
{
    [TestClass]
    public class AzureBlobBrokerTests : TestBase
    {
        private const string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=test;AccountKey=3w1OwI/N6dqvmN0Iaa0/y6zlqL81H42K/mfIbIIKeFQkNpHSNvOcnWpucvrX5rbKGm+WKEUxaOZikeTMWpXfxA==";

        [TestMethod]
        public void Blob_CreateResourceToken_WithNullConnectionString_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker().CreateResourceToken(null, new BlobParameters { Name = "blob", Container = "container" }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithEmptyConnectionString_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker().CreateResourceToken(string.Empty, new BlobParameters { Name = "blob", Container = "container" }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithWhitespaceConnectionString_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker().CreateResourceToken(" ", new BlobParameters { Name = "blob", Container = "container" }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithNullParameters_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker().CreateResourceToken(ConnectionString, null));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithResourceParameters_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker().CreateResourceToken(ConnectionString, new ResourceParameters { Name = "blob" }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithNullContainerName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker().CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = null }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithEmptyContainerName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker().CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = string.Empty }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithWhitespaceContainerName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker().CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = " " }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithNullBlobName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker().CreateResourceToken(ConnectionString, new BlobParameters { Name = null, Container = "container" }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithEmptyBlobName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker().CreateResourceToken(ConnectionString, new BlobParameters { Name = string.Empty, Container = "container" }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithWhitespaceBlobName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker().CreateResourceToken(ConnectionString, new BlobParameters { Name = " ", Container = "container" }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_ReturnsCorrectHostPath()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act.
            ResourceToken token = broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Assert.
            Assert.IsNotNull(token);

            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("https://test.blob.core.windows.net/container/blob", parts.HostName);
        }

        [TestMethod]
        public void Blob_CreateResourceAsync_WithExpirationDate_ReturnsCorrectExpirationDate()
        {
            // Setup
            DateTime expiration = new DateTime(2199, 3, 12, 1, 2, 3, DateTimeKind.Utc);
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act.
            ResourceToken token = broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read, Expiration = expiration });

            // Assert.
            Assert.IsNotNull(token);

            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("2199-03-12T01%3A02%3A03Z", parts.Value("se"));
        }

        [TestMethod]
        public void Blob_CreateResourceAsync_WithNoExpirationDate_ReturnsNoExpirationDate()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act.
            ResourceToken token = broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read });

            // Assert.
            Assert.IsNotNull(token);

            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual(null, parts.Value("se"));
        }

        [TestMethod]
        public void Blob_CreateResourceAsync_ReturnsExpectedStartDate()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act.
            ResourceToken token = broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read });

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
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act
            ResourceToken token = broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("r", parts.Value("sp"));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithWritePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act
            ResourceToken token = broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Write, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("w", parts.Value("sp"));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithReadWritePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act
            ResourceToken token = broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.ReadWrite, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rw", parts.Value("sp"));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithReadAndWritePermissions_ReturnsCorrectAccess()
        {
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act
            ResourceToken token = broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Write, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rw", parts.Value("sp"));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithReadAddUpdateDeletePermissions_ReturnsCorrectAccess()
        {
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act
            ResourceToken token = broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Add | ResourcePermissions.Update | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rw", parts.Value("sp"));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithAddUpdateDeletePermissions_ReturnsCorrectAccess()
        {
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act
            ResourceToken token = broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Add | ResourcePermissions.Update | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("w", parts.Value("sp"));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithAddPermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Add, Expiration = DateTime.Now + TimeSpan.FromDays(1) }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithReadAddPermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Add, Expiration = DateTime.Now + TimeSpan.FromDays(1) }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithUpdatePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Update, Expiration = DateTime.Now + TimeSpan.FromDays(1) }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithReadUpdatePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Update, Expiration = DateTime.Now + TimeSpan.FromDays(1) }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithDeletePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithReadDeletePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithAddUpdatePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Add | ResourcePermissions.Update, Expiration = DateTime.Now + TimeSpan.FromDays(1) }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithReadAddUpdatePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Add | ResourcePermissions.Update, Expiration = DateTime.Now + TimeSpan.FromDays(1) }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithAddDeletePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Add | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithReadAddDeletePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Add | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithUpdateDeletePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Update | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithReadUpdateDeletePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Update | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) }));
        }

        [TestMethod]
        public void Blob_CreateResourceToken_WithProcessPermissions_IgnoresProcessPermissions()
        {
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act
            ResourceToken token = broker.CreateResourceToken(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Process, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("r", parts.Value("sp"));
        }

        [TestMethod]
        public void Blob_ExtractParameters_ReturnsBlobToken()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();

            // Act
            BlobParameters parameters = (BlobParameters)broker.ExtractParameters(this.GenerateBlobJson());

            // Assert
            Assert.AreEqual("container", parameters.Container);
            Assert.AreEqual("blob", parameters.Name);
            Assert.AreEqual(ResourcePermissions.ReadWrite, parameters.Permissions);
            Assert.IsTrue(parameters.Expiration > DateTime.UtcNow - TimeSpan.FromSeconds(1) && parameters.Expiration < DateTime.UtcNow + TimeSpan.FromSeconds(1));
        }

        [TestMethod]
        public void Blob_ExtractConnectionString_AppSettingsBlobConnectionStringIsNull_ThrowsArgumentException()
        {
            AzureBlobBroker broker = new AzureBlobBroker();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerBlobConnectionString", null);
            this.ExpectException<InvalidOperationException>(() => broker.ExtractConnectionString(appSettings));
        }

        [TestMethod]
        public void Blob_ExtractConnectionString_AppSettingsConnectionString_ReturnsBlobConnectionString()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", ConnectionString);

            // Act
            string c = broker.ExtractConnectionString(appSettings);

            // Assert
            Assert.AreEqual(c, ConnectionString);
        }

        [TestMethod]
        public void Blob_ExtractConnectionString_AppSettingsBlobConnectionString_ReturnsBlobConnectionString()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerBlobConnectionString", ConnectionString);

            // Act
            string c = broker.ExtractConnectionString(appSettings);

            // Assert
            Assert.AreEqual(c, ConnectionString);
        }

        [TestMethod]
        public void Blob_ExtractConnectionString_WithTableConnectionString_ThrowsException()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerTableConnectionString", ConnectionString);

            // Act
            this.ExpectException<InvalidOperationException>(() => broker.ExtractConnectionString(appSettings));
        }

        [TestMethod]
        public void Blob_ExtractConnectionString_WithQueueConnectionString_ThrowsException()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerQueueConnectionString", ConnectionString);

            // Act
            this.ExpectException<InvalidOperationException>(() => broker.ExtractConnectionString(appSettings));
        }

        private JToken GenerateBlobJson()
        {
            JToken token = new JObject();
            token["container"] = "container";
            token["name"] = "blob";
            token["permissions"] = "rw";
            token["expiry"] = DateTime.UtcNow.ToString();
            return token;
        }

        private ResourceParameters GenerateBlobParameters()
        {
            return new BlobParameters
            {
                Container = "container",
                Name = "blob",
                Permissions = ResourcePermissions.ReadWrite,
                Expiration = DateTime.UtcNow
            };
        }
    }
}