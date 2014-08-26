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
    public class AzureResourceBrokerTests : TestBase
    {
        private const string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=test;AccountKey=3w1OwI/N6dqvmN0Iaa0/y6zlqL81H42K/mfIbIIKeFQkNpHSNvOcnWpucvrX5rbKGm+WKEUxaOZikeTMWpXfxA==";

        [TestMethod]
        public void AzureResourceBroker_ExtractParameters_WithNullJsonParameters_ThrowsArgumentException()
        {
            TestResourceBroker broker = new TestResourceBroker();
            this.ExpectException<ArgumentNullException>(() => broker.ExtractParameters((JToken)null));
        }

        [TestMethod]
        public void AzureResourceBroker_ExtractParameters_WithMissingName_ThrowsException()
        {
            // Setup
            TestResourceBroker broker = new TestResourceBroker();
            JToken parameters = this.GenerateTableJson();
            parameters["name"] = string.Empty;

            // Act
            this.ExpectException<HttpResponseException>(() => broker.ExtractParameters(parameters));
        }

        [TestMethod]
        public void AzureResourceBroker_ExtractParameters_WithMissingPermissions_ThrowsException()
        {
            // Setup
            TestResourceBroker broker = new TestResourceBroker();
            JToken parameters = this.GenerateTableJson();
            parameters["permissions"] = string.Empty;

            // Act
            this.ExpectException<HttpResponseException>(() => broker.ExtractParameters(parameters));
        }

        [TestMethod]
        public void AzureResourceBroker_ExtractParameters_WithInvalidPermissions_ThrowsException()
        {
            // Setup
            TestResourceBroker broker = new TestResourceBroker();
            JToken parameters = this.GenerateTableJson();
            parameters["permissions"] = "asdfadsf";

            // Act
            this.ExpectException<HttpResponseException>(() => broker.ExtractParameters(parameters));
        }

        [TestMethod]
        public void AzureResourceBroker_ExtractParameters_WithMissingDate_ThrowsException()
        {
            // Setup
            TestResourceBroker broker = new TestResourceBroker();
            JToken parameters = this.GenerateTableJson();
            parameters["expiry"] = string.Empty;

            // Act
            this.ExpectException<HttpResponseException>(() => broker.ExtractParameters(parameters));
        }

        [TestMethod]
        public void AzureResourceBroker_ExtractParameters_WithInvalidDate_ThrowsException()
        {
            // Setup
            TestResourceBroker broker = new TestResourceBroker();
            JToken parameters = this.GenerateTableJson();
            parameters["expiry"] = "asfasdf";

            // Act
            this.ExpectException<HttpResponseException>(() => broker.ExtractParameters(parameters));
        }

        [TestMethod]
        public void AzureResourceBroker_ExtractParameters_ReturnsResourceParameters()
        {
            // Setup
            TestResourceBroker broker = new TestResourceBroker();

            // Act
            ResourceParameters parameters = broker.ExtractParameters(this.GenerateTableJson());

            // Assert
            Assert.AreEqual("table", parameters.Name);
            Assert.AreEqual(ResourcePermissions.ReadWrite, parameters.Permissions);
            Assert.IsTrue(parameters.Expiration > DateTime.UtcNow - TimeSpan.FromSeconds(1) && parameters.Expiration < DateTime.UtcNow + TimeSpan.FromSeconds(1));
        }

        private JToken GenerateTableJson()
        {
            JToken token = new JObject();
            token["name"] = "table";
            token["permissions"] = "rw";
            token["expiry"] = DateTime.UtcNow.ToString();
            return token;
        }

        private JToken GenerateQueueJson()
        {
            JToken token = new JObject();
            token["name"] = "table";
            token["permissions"] = "raup";
            token["expiry"] = DateTime.UtcNow.ToString();
            return token;
        }

        private ResourceParameters GenerateTableParameters()
        {
            return new ResourceParameters
            {
                Name = "table",
                Permissions = ResourcePermissions.ReadWrite,
                Expiration = DateTime.UtcNow
            };
        }

        private ResourceParameters GenerateQueueParameters()
        {
            return new ResourceParameters
            {
                Name = "queue",
                Permissions = ResourcePermissions.ReadWrite | ResourcePermissions.Process,
                Expiration = DateTime.UtcNow
            };
        }

        public class TestResourceBroker : AzureResourceBroker
        {
            public override ResourcePermissions AllowedPermissions
            {
                get { return ResourcePermissions.ReadWrite; }
            }

            public override ResourceToken CreateResourceToken(string connectionString, Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models.ResourceParameters parameters)
            {
                throw new NotImplementedException();
            }

            public override string ExtractConnectionString(IDictionary<string, string> settings)
            {
                throw new NotImplementedException();
            }
        }
    }
}