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
    public class AzureTableBrokerTests : TestBase
    {
        private const string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=test;AccountKey=3w1OwI/N6dqvmN0Iaa0/y6zlqL81H42K/mfIbIIKeFQkNpHSNvOcnWpucvrX5rbKGm+WKEUxaOZikeTMWpXfxA==";

        [TestMethod]
        public void Table_Create_WithNullConnectionString_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureTableBroker(null, new ResourceParameters { Name = "table" }));
        }

        [TestMethod]
        public void Table_Create_WithEmptyConnectionString_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureTableBroker("", new ResourceParameters { Name = "table" }));
        }

        [TestMethod]
        public void Table_Create_WithWhitespaceConnectionString_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureTableBroker(" ", new ResourceParameters { Name = "table" }));
        }

        [TestMethod]
        public void Table_Create_WithNullParameters_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureTableBroker(ConnectionString, null));
        }

        [TestMethod]
        public void Table_Create_WithNullTableName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureTableBroker(ConnectionString, new ResourceParameters { Name = null }));
        }

        [TestMethod]
        public void Table_Create_WithEmptyTableName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "" }));
        }

        [TestMethod]
        public void Table_Create_WithWhitespaceTableName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureTableBroker(ConnectionString, new ResourceParameters { Name = " " }));
        }

        [TestMethod]
        public void Table_CreateResourceToken_ReturnsCorrectHostPath()
        {
            // Setup
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Read, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act.
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);

            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("https://test.table.core.windows.net/table", parts.HostName);
        }

        [TestMethod]
        public void Table_CreateResourceToken_WithExpirationDate_ReturnsCorrectExpirationDate()
        {
            // Setup
            DateTime expiration = new DateTime(2199, 3, 12);
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Read, Expiration = expiration });

            // Act.
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);

            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("2199-03-12T07%3A00%3A00Z", parts.Value("se"));
        }

        [TestMethod]
        public void Table_CreateResourceToken_WithNoExpirationDate_ReturnsNoExpirationDate()
        {
            // Setup
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Read });

            // Act.
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);

            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual(null, parts.Value("se"));
        }

        [TestMethod]
        public void Table_CreateResourceToken_ReturnsExpectedStartDate()
        {
            // Setup
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Read });

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
        public void Table_CreateResourceToken_WithReadPermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Read, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("r", parts.Value("sp"));
        }

        [TestMethod]
        public void Table_CreateResourceToken_WithWritePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Write, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("aud", parts.Value("sp"));
        }

        [TestMethod]
        public void Table_CreateResourceToken_WithReadWritePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.ReadWrite, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("raud", parts.Value("sp"));
        }

        [TestMethod]
        public void Table_CreateResourceToken_WithReadAndWritePermissions_ReturnsCorrectAccess()
        {
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Read | ResourcePermissions.Write, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("raud", parts.Value("sp"));
        }

        [TestMethod]
        public void Table_CreateResourceToken_WithReadAddUpdateDeletePermissions_ReturnsCorrectAccess()
        {
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Read | ResourcePermissions.Add | ResourcePermissions.Update | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("raud", parts.Value("sp"));
        }

        [TestMethod]
        public void Table_CreateResourceToken_WithAddUpdateDeletePermissions_ReturnsCorrectAccess()
        {
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Add | ResourcePermissions.Update | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("aud", parts.Value("sp"));
        }

        [TestMethod]
        public void Table_CreateResourceToken_WithAddPermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Add, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("a", parts.Value("sp"));
        }

        [TestMethod]
        public void Table_CreateResourceToken_WithReadAddPermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Read | ResourcePermissions.Add, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("ra", parts.Value("sp"));
        }

        [TestMethod]
        public void Table_CreateResourceToken_WithUpdatePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Update, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("u", parts.Value("sp"));
        }

        [TestMethod]
        public void Table_CreateResourceToken_WithReadUpdatePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Read | ResourcePermissions.Update, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("ru", parts.Value("sp"));
        }

        [TestMethod]
        public void Table_CreateResourceToken_WithDeletePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("d", parts.Value("sp"));
        }

        [TestMethod]
        public void Table_CreateResourceToken_WithReadDeletePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Read | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rd", parts.Value("sp"));
        }

        [TestMethod]
        public void Table_CreateResourceToken_WithAddUpdatePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Add | ResourcePermissions.Update, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("au", parts.Value("sp"));
        }

        [TestMethod]
        public void Table_CreateResourceToken_WithReadAddUpdatePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Read | ResourcePermissions.Add | ResourcePermissions.Update, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rau", parts.Value("sp"));
        }

        [TestMethod]
        public void Table_CreateResourceToken_WithAddDeletePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Add | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("ad", parts.Value("sp"));
        }

        [TestMethod]
        public void Table_CreateResourceToken_WithReadAddDeletePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Read | ResourcePermissions.Add | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rad", parts.Value("sp"));
        }

        [TestMethod]
        public void Table_CreateResourceToken_WithUpdateDeletePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Update | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("ud", parts.Value("sp"));
        }

        [TestMethod]
        public void Table_CreateResourceToken_WithReadUpdateDeletePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureTableBroker broker = new AzureTableBroker(ConnectionString, new ResourceParameters { Name = "table", Permissions = ResourcePermissions.Read | ResourcePermissions.Update | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rud", parts.Value("sp"));
        }
    }
}