using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Brokers;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Providers;
using Moq;
using Newtonsoft.Json.Linq;

namespace Test.WindowsAzure.Mobile.Service.ResourceBroker
{
    [TestClass]
    public class AzureStorageBrokerTests : TestBase
    {
        private const string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=test;AccountKey=3w1OwI/N6dqvmN0Iaa0/y6zlqL81H42K/mfIbIIKeFQkNpHSNvOcnWpucvrX5rbKGm+WKEUxaOZikeTMWpXfxA==";

        [TestMethod]
        public void AzureStorageBroker_ExtractConnectionString_NullAppSettings_ThrowsArgumentException()
        {
            TestStorageBroker broker = new TestStorageBroker();
            this.ExpectException<ArgumentNullException>(() => broker.ExtractConnectionString((IDictionary<string, string>)null));
        }

        [TestMethod]
        public void AzureStorageBroker_ExtractConnectionString_EmptyAppSettings_ThrowsArgumentException()
        {
            TestStorageBroker broker = new TestStorageBroker();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            this.ExpectException<InvalidOperationException>(() => broker.ExtractConnectionString(appSettings));
        }

        [TestMethod]
        public void AzureStorageBroker_ExtractConnectionString_AppSettingsConnectionStringIsNull_ThrowsArgumentException()
        {
            TestStorageBroker broker = new TestStorageBroker();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", null);
            this.ExpectException<InvalidOperationException>(() => broker.ExtractConnectionString(appSettings));
        }

        [TestMethod]
        public void AzureStorageBroker_ExtractConnectionString_AppSettingsConnectionString_ReturnsTableConnectionString()
        {
            // Setup
            TestStorageBroker broker = new TestStorageBroker();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", ConnectionString);

            // Act
            string c = broker.ExtractConnectionString(appSettings);

            // Assert
            Assert.AreEqual(c, ConnectionString);
        }

        public class TestStorageBroker : AzureStorageBroker
        {
            public override ResourcePermissions AllowedPermissions
            {
                get { return ResourcePermissions.Read | ResourcePermissions.Update; }
            }

            public override ResourceToken CreateResourceToken(string connectionString, ResourceParameters parameters)
            {
                throw new NotImplementedException();
            }
        }
    }
}