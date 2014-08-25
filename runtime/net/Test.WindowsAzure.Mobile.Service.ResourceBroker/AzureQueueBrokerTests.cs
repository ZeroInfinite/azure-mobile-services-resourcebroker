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
    public class AzureQueueBrokerTests : TestBase
    {
        private const string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=test;AccountKey=3w1OwI/N6dqvmN0Iaa0/y6zlqL81H42K/mfIbIIKeFQkNpHSNvOcnWpucvrX5rbKGm+WKEUxaOZikeTMWpXfxA==";

        [TestMethod]
        public void Queue_Create_WithNullConnectionString_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureQueueBroker(null, new ResourceParameters { Name = "queue" }));
        }

        [TestMethod]
        public void Queue_Create_WithEmptyConnectionString_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureQueueBroker(string.Empty, new ResourceParameters { Name = "queue" }));
        }

        [TestMethod]
        public void Queue_Create_WithWhitespaceConnectionString_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureQueueBroker(" ", new ResourceParameters { Name = "queue" }));
        }

        [TestMethod]
        public void Queue_Create_WithNullParameters_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureQueueBroker(ConnectionString, null));
        }

        [TestMethod]
        public void Queue_Create_WithNullQueueName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = null }));
        }

        [TestMethod]
        public void Queue_Create_WithEmptyQueueName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = string.Empty }));
        }

        [TestMethod]
        public void Queue_Create_WithWhitespaceQueueName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = " " }));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_ReturnsCorrectHostPath()
        {
            // Setup
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Read, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act.
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);

            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("https://test.queue.core.windows.net/queue", parts.HostName);
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithExpirationDate_ReturnsCorrectExpirationDate()
        {
            // Setup
            DateTime expiration = new DateTime(2199, 3, 12, 1, 2, 3, DateTimeKind.Utc);
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Read, Expiration = expiration });

            // Act.
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);

            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("2199-03-12T01%3A02%3A03Z", parts.Value("se"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithNoExpirationDate_ReturnsNoExpirationDate()
        {
            // Setup
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Read });

            // Act.
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);

            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual(null, parts.Value("se"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_ReturnsExpectedStartDate()
        {
            // Setup
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Read });

            // Act.
            ResourceToken token = broker.CreateResourceToken();

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
        public void Queue_CreateResourceToken_WithReadPermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Read, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("r", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithWritePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Write, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("au", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithReadWritePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.ReadWrite, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rau", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithReadAndWritePermissions_ReturnsCorrectAccess()
        {
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Read | ResourcePermissions.Write, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rau", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithReadAndProcessPermissions_ReturnsCorrectAccess()
        {
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Read | ResourcePermissions.Process, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rp", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithReadAddUpdateDeletePermissions_ReturnsCorrectAccess()
        {
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Read | ResourcePermissions.Add | ResourcePermissions.Update | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rau", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithReadAddUpdateProcessPermissions_ReturnsCorrectAccess()
        {
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Read | ResourcePermissions.Add | ResourcePermissions.Update | ResourcePermissions.Process, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("raup", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithReadAddUpdateDeleteProcessPermissions_ReturnsCorrectAccess()
        {
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Read | ResourcePermissions.Add | ResourcePermissions.Update | ResourcePermissions.Delete | ResourcePermissions.Process, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("raup", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithAddUpdateProcessPermissions_ReturnsCorrectAccess()
        {
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Add | ResourcePermissions.Update | ResourcePermissions.Process, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("aup", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithAddPermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Add, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("a", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithReadAddPermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Read | ResourcePermissions.Add, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("ra", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithUpdatePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Update, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("u", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithReadUpdatePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Read | ResourcePermissions.Update, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("ru", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithProcessPermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Process, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("p", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithReadProcessPermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Read | ResourcePermissions.Process, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rp", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithAddUpdatePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Add | ResourcePermissions.Update, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("au", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithReadAddUpdatePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Read | ResourcePermissions.Add | ResourcePermissions.Update, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rau", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithAddProcessPermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Add | ResourcePermissions.Process, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("ap", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithReadAddProcessPermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Read | ResourcePermissions.Add | ResourcePermissions.Process, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rap", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithUpdateProcessPermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Update | ResourcePermissions.Process, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("up", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithReadUpdateProcessPermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Read | ResourcePermissions.Update | ResourcePermissions.Process, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rup", parts.Value("sp"));
        }

        [TestMethod]
        public void Queue_CreateResourceToken_WithDeletePermissions_IgnoresProcessPermissions()
        {
            // Setup
            AzureQueueBroker broker = new AzureQueueBroker(ConnectionString, new ResourceParameters { Name = "queue", Permissions = ResourcePermissions.Read | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("r", parts.Value("sp"));
        }
    }
}