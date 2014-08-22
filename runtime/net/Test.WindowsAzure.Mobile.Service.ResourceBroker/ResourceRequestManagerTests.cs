using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Brokers;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Providers;
using Moq;
using Newtonsoft.Json.Linq;

namespace Test.WindowsAzure.Mobile.Service.ResourceBroker
{
    [TestClass]
    public class ResourceRequestManagerTests : TestBase
    {
        private const string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=test;AccountKey=3w1OwI/N6dqvmN0Iaa0/y6zlqL81H42K/mfIbIIKeFQkNpHSNvOcnWpucvrX5rbKGm+WKEUxaOZikeTMWpXfxA==";

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_WithNullResourceType_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentException>(() => m.GenerateToken(null, this.GenerateTableJson(), ConnectionString));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_WithEmptyResourceType_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentException>(() => m.GenerateToken(string.Empty, this.GenerateTableJson(), ConnectionString));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_WithWhitespaceResourceType_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentException>(() => m.GenerateToken(" ", this.GenerateTableJson(), ConnectionString));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_WithNullJsonParameters_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentException>(() => m.GenerateToken("table", (JToken)null, ConnectionString));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_WithNullJsonParameters_ThrowsArgumentException2()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentException>(() => m.GenerateToken(ResourceType.Table, (JToken)null, ConnectionString));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_WithNullResourceParameters_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentException>(() => m.GenerateToken(ResourceType.Table, (ResourceParameters)null, ConnectionString));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_WithNullConnectionString_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentException>(() => m.GenerateToken("table", this.GenerateTableJson(), (string)null));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_WithNullConnectionString_ThrowsArgumentException2()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentException>(() => m.GenerateToken(ResourceType.Table, this.GenerateTableJson(), (string)null));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_WithNullConnectionString_ThrowsArgumentException3()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentException>(() => m.GenerateToken(ResourceType.Table, this.GenerateTableParameters(), (string)null));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_WithEmptyConnectionString_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentException>(() => m.GenerateToken("table", this.GenerateTableJson(), string.Empty));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_WithEmptyConnectionString_ThrowsArgumentException2()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentException>(() => m.GenerateToken(ResourceType.Table, this.GenerateTableJson(), string.Empty));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_WithEmptyConnectionString_ThrowsArgumentException3()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentException>(() => m.GenerateToken(ResourceType.Table, this.GenerateTableParameters(), string.Empty));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_WithWhitespaceConnectionString_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentException>(() => m.GenerateToken("table", this.GenerateTableJson(), " "));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_WithWhitespaceConnectionString_ThrowsArgumentException2()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentException>(() => m.GenerateToken(ResourceType.Table, this.GenerateTableJson(), " "));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_WithWhitespaceConnectionString_ThrowsArgumentException3()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentException>(() => m.GenerateToken(ResourceType.Table, this.GenerateTableParameters(), " "));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_ReturnsTableToken()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();

            // Act
            ResourceToken token = m.GenerateToken("table", this.GenerateTableJson(), ConnectionString);

            // Assert
            Assert.IsNotNull(token.Uri);
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_ReturnsTableToken2()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();

            // Act
            ResourceToken token = m.GenerateToken(ResourceType.Table, this.GenerateTableJson(), ConnectionString);

            // Assert
            Assert.IsNotNull(token.Uri);
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_ReturnsTableToken3()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();

            // Act
            ResourceToken token = m.GenerateToken(ResourceType.Table, this.GenerateTableParameters(), ConnectionString);

            // Assert
            Assert.IsNotNull(token.Uri);
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_ReturnsBlobToken()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();

            // Act
            ResourceToken token = m.GenerateToken("blob", this.GenerateBlobJson(), ConnectionString);

            // Assert
            Assert.IsNotNull(token.Uri);
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_ReturnsBlobToken2()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();

            // Act
            ResourceToken token = m.GenerateToken(ResourceType.Blob, this.GenerateBlobJson(), ConnectionString);

            // Assert
            Assert.IsNotNull(token.Uri);
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_ReturnsBlobToken3()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();

            // Act
            ResourceToken token = m.GenerateToken(ResourceType.Blob, this.GenerateBlobParameters(), ConnectionString);

            // Assert
            Assert.IsNotNull(token.Uri);
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_NullAppSettings_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentNullException>(() => m.GenerateToken("table", this.GenerateTableJson(), (IDictionary<string, string>)null));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_NullAppSettings_ThrowsArgumentException2()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentNullException>(() => m.GenerateToken(ResourceType.Table, this.GenerateTableJson(), (IDictionary<string, string>)null));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_NullAppSettings_ThrowsArgumentException3()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentNullException>(() => m.GenerateToken(ResourceType.Table, this.GenerateTableParameters(), (IDictionary<string, string>)null));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_EmptyAppSettings_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            this.ExpectException<InvalidOperationException>(() => m.GenerateToken("table", this.GenerateTableJson(), appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_EmptyAppSettings_ThrowsArgumentException2()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            this.ExpectException<InvalidOperationException>(() => m.GenerateToken(ResourceType.Table, this.GenerateTableJson(), appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_EmptyAppSettings_ThrowsArgumentException3()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            this.ExpectException<InvalidOperationException>(() => m.GenerateToken(ResourceType.Table, this.GenerateTableParameters(), appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsConnectionStringIsNull_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", null);
            this.ExpectException<InvalidOperationException>(() => m.GenerateToken("table", this.GenerateTableJson(), appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsConnectionStringIsNull_ThrowsArgumentException2()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", null);
            this.ExpectException<InvalidOperationException>(() => m.GenerateToken(ResourceType.Table, this.GenerateTableJson(), appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsConnectionStringIsNull_ThrowsArgumentException3()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", null);
            this.ExpectException<InvalidOperationException>(() => m.GenerateToken(ResourceType.Table, this.GenerateTableParameters(), appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsTableConnectionStringIsNull_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerTableConnectionString", null);
            this.ExpectException<InvalidOperationException>(() => m.GenerateToken("table", this.GenerateTableJson(), appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsTableConnectionStringIsNull_ThrowsArgumentException2()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerTableConnectionString", null);
            this.ExpectException<InvalidOperationException>(() => m.GenerateToken(ResourceType.Table, this.GenerateTableJson(), appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsTableConnectionStringIsNull_ThrowsArgumentException3()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerTableConnectionString", null);
            this.ExpectException<InvalidOperationException>(() => m.GenerateToken(ResourceType.Table, this.GenerateTableParameters(), appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsBlobConnectionStringIsNull_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerBlobConnectionString", null);
            this.ExpectException<InvalidOperationException>(() => m.GenerateToken("blob", this.GenerateBlobJson(), appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsBlobConnectionStringIsNull_ThrowsArgumentException2()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerBlobConnectionString", null);
            this.ExpectException<InvalidOperationException>(() => m.GenerateToken(ResourceType.Blob, this.GenerateBlobJson(), appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsBlobConnectionStringIsNull_ThrowsArgumentException3()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerBlobConnectionString", null);
            this.ExpectException<InvalidOperationException>(() => m.GenerateToken(ResourceType.Blob, this.GenerateBlobParameters(), appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsConnectionString_ReturnsTableToken()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", ConnectionString);

            // Act
            ResourceToken token = m.GenerateToken("table", this.GenerateTableJson(), appSettings);

            // Assert
            Assert.IsNotNull(token.Uri);
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsConnectionString_ReturnsTableToken2()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", ConnectionString);

            // Act
            ResourceToken token = m.GenerateToken(ResourceType.Table, this.GenerateTableJson(), appSettings);

            // Assert
            Assert.IsNotNull(token.Uri);
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsConnectionString_ReturnsTableToken3()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", ConnectionString);

            // Act
            ResourceToken token = m.GenerateToken(ResourceType.Table, this.GenerateTableParameters(), appSettings);

            // Assert
            Assert.IsNotNull(token.Uri);
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsConnectionString_ReturnsBlobToken()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", ConnectionString);

            // Act
            ResourceToken token = m.GenerateToken("Blob", this.GenerateBlobJson(), appSettings);

            // Assert
            Assert.IsNotNull(token.Uri);
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsConnectionString_ReturnsBlobToken2()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", ConnectionString);

            // Act
            ResourceToken token = m.GenerateToken(ResourceType.Blob, this.GenerateBlobJson(), appSettings);

            // Assert
            Assert.IsNotNull(token.Uri);
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsConnectionString_ReturnsBlobToken3()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", ConnectionString);

            // Act
            ResourceToken token = m.GenerateToken(ResourceType.Blob, this.GenerateBlobParameters(), appSettings);

            // Assert
            Assert.IsNotNull(token.Uri);
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsTableConnectionString_ReturnsTableToken()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerTableConnectionString", ConnectionString);

            // Act
            ResourceToken token = m.GenerateToken("table", this.GenerateTableJson(), appSettings);

            // Assert
            Assert.IsNotNull(token.Uri);
        }

        public void ResourceRequestManager_GenerateToken_AppSettingsTableConnectionString_ReturnsTableToken2()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerTableConnectionString", ConnectionString);

            // Act
            ResourceToken token = m.GenerateToken(ResourceType.Table, this.GenerateTableJson(), appSettings);

            // Assert
            Assert.IsNotNull(token.Uri);
        }

        public void ResourceRequestManager_GenerateToken_AppSettingsTableConnectionString_ReturnsTableToken3()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerTableConnectionString", ConnectionString);

            // Act
            ResourceToken token = m.GenerateToken(ResourceType.Table, this.GenerateTableParameters(), appSettings);

            // Assert
            Assert.IsNotNull(token.Uri);
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsBlobConnectionString_ReturnsBlobToken()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerBlobConnectionString", ConnectionString);

            // Act
            ResourceToken token = m.GenerateToken("blob", this.GenerateBlobJson(), appSettings);

            // Assert
            Assert.IsNotNull(token.Uri);
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsBlobConnectionString_ReturnsBlobToken2()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerBlobConnectionString", ConnectionString);

            // Act
            ResourceToken token = m.GenerateToken(ResourceType.Blob, this.GenerateBlobJson(), appSettings);

            // Assert
            Assert.IsNotNull(token.Uri);
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsBlobConnectionString_ReturnsBlobToken3()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerBlobConnectionString", ConnectionString);

            // Act
            ResourceToken token = m.GenerateToken(ResourceType.Blob, this.GenerateBlobParameters(), appSettings);

            // Assert
            Assert.IsNotNull(token.Uri);
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsTableConnectionString_WithBlobRequest_ThrowsException()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerTableConnectionString", ConnectionString);

            // Act
            this.ExpectException<InvalidOperationException>(() => m.GenerateToken("blob", this.GenerateBlobJson(), appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsTableConnectionString_WithBlobRequest_ThrowsException2()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerTableConnectionString", ConnectionString);

            // Act
            this.ExpectException<InvalidOperationException>(() => m.GenerateToken(ResourceType.Blob, this.GenerateBlobJson(), appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsTableConnectionString_WithBlobRequest_ThrowsException3()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerTableConnectionString", ConnectionString);

            // Act
            this.ExpectException<InvalidOperationException>(() => m.GenerateToken(ResourceType.Blob, this.GenerateBlobParameters(), appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsBlobConnectionString_WithTableRequest_ThrowsException()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerBlobConnectionString", ConnectionString);

            // Act
            this.ExpectException<InvalidOperationException>(() => m.GenerateToken("table", this.GenerateTableJson(), appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsBlobConnectionString_WithTableRequest_ThrowsException2()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerBlobConnectionString", ConnectionString);

            // Act
            this.ExpectException<InvalidOperationException>(() => m.GenerateToken(ResourceType.Table, this.GenerateTableJson(), appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GenerateToken_AppSettingsBlobConnectionString_WithTableRequest_ThrowsException3()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerBlobConnectionString", ConnectionString);

            // Act
            this.ExpectException<InvalidOperationException>(() => m.GenerateToken(ResourceType.Table, this.GenerateTableParameters(), appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_NullResourceType_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", null);
            this.ExpectException<ArgumentException>(() => m.GetConnectionString(null, appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_EmptyResourceType_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", null);
            this.ExpectException<ArgumentException>(() => m.GetConnectionString(string.Empty, appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_WhitespaceResourceType_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", null);
            this.ExpectException<ArgumentException>(() => m.GetConnectionString(" ", appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_NullAppSettings_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentNullException>(() => m.GetConnectionString("table", (IDictionary<string, string>)null));
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_NullAppSettings_ThrowsArgumentException2()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentNullException>(() => m.GetConnectionString(ResourceType.Table, (IDictionary<string, string>)null));
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_EmptyAppSettings_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            this.ExpectException<InvalidOperationException>(() => m.GetConnectionString("table", appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_EmptyAppSettings_ThrowsArgumentException2()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            this.ExpectException<InvalidOperationException>(() => m.GetConnectionString(ResourceType.Table, appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_AppSettingsConnectionStringIsNull_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", null);
            this.ExpectException<InvalidOperationException>(() => m.GetConnectionString("table", appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_AppSettingsConnectionStringIsNull_ThrowsArgumentException2()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", null);
            this.ExpectException<InvalidOperationException>(() => m.GetConnectionString(ResourceType.Table, appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_AppSettingsTableConnectionStringIsNull_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerTableConnectionString", null);
            this.ExpectException<InvalidOperationException>(() => m.GetConnectionString("table", appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_AppSettingsTableConnectionStringIsNull_ThrowsArgumentException2()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerTableConnectionString", null);
            this.ExpectException<InvalidOperationException>(() => m.GetConnectionString(ResourceType.Table, appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_AppSettingsBlobConnectionStringIsNull_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerBlobConnectionString", null);
            this.ExpectException<InvalidOperationException>(() => m.GetConnectionString("blob", appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_AppSettingsBlobConnectionStringIsNull_ThrowsArgumentException2()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerBlobConnectionString", null);
            this.ExpectException<InvalidOperationException>(() => m.GetConnectionString(ResourceType.Blob, appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_AppSettingsConnectionString_ReturnsTableConnectionString()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", ConnectionString);

            // Act
            string c = m.GetConnectionString("table", appSettings);

            // Assert
            Assert.AreEqual(c, ConnectionString);
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_AppSettingsConnectionString_ReturnsTableConnectionString2()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", ConnectionString);

            // Act
            string c = m.GetConnectionString(ResourceType.Table, appSettings);

            // Assert
            Assert.AreEqual(c, ConnectionString);
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_AppSettingsConnectionString_ReturnsBlobConnectionString()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", ConnectionString);

            // Act
            string c = m.GetConnectionString("Blob", appSettings);

            // Assert
            Assert.AreEqual(c, ConnectionString);
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_AppSettingsConnectionString_ReturnsBlobConnectionString2()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerStorageConnectionString", ConnectionString);

            // Act
            string c = m.GetConnectionString(ResourceType.Blob, appSettings);

            // Assert
            Assert.AreEqual(c, ConnectionString);
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_AppSettingsTableConnectionString_ReturnsTableConnectionString()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerTableConnectionString", ConnectionString);

            // Act
            string c = m.GetConnectionString("table", appSettings);

            // Assert
            Assert.AreEqual(c, ConnectionString);
        }

        public void ResourceRequestManager_GetConnectionString_AppSettingsTableConnectionString_ReturnsTableConnectionString2()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerTableConnectionString", ConnectionString);

            // Act
            string c = m.GetConnectionString(ResourceType.Table, appSettings);

            // Assert
            Assert.AreEqual(c, ConnectionString);
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_AppSettingsBlobConnectionString_ReturnsBlobConnectionString()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerBlobConnectionString", ConnectionString);

            // Act
            string c = m.GetConnectionString("blob", appSettings);

            // Assert
            Assert.AreEqual(c, ConnectionString);
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_AppSettingsBlobConnectionString_ReturnsBlobConnectionString2()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerBlobConnectionString", ConnectionString);

            // Act
            string c = m.GetConnectionString(ResourceType.Blob, appSettings);

            // Assert
            Assert.AreEqual(c, ConnectionString);
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_AppSettingsTableConnectionString_WithBlobRequest_ThrowsException()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerTableConnectionString", ConnectionString);

            // Act
            this.ExpectException<InvalidOperationException>(() => m.GetConnectionString("blob", appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_AppSettingsTableConnectionString_WithBlobRequest_ThrowsException2()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerTableConnectionString", ConnectionString);

            // Act
            this.ExpectException<InvalidOperationException>(() => m.GetConnectionString(ResourceType.Blob, appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_AppSettingsBlobConnectionString_WithTableRequest_ThrowsException()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerBlobConnectionString", ConnectionString);

            // Act
            this.ExpectException<InvalidOperationException>(() => m.GetConnectionString("table", appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_GetConnectionString_AppSettingsBlobConnectionString_WithTableRequest_ThrowsException2()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            appSettings.Add("ResourceBrokerBlobConnectionString", ConnectionString);

            // Act
            this.ExpectException<InvalidOperationException>(() => m.GetConnectionString(ResourceType.Table, appSettings));
        }

        [TestMethod]
        public void ResourceRequestManager_ParseTokenParameters_WithNullResourceType_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentException>(() => m.ParseTokenParameters(null, this.GenerateTableJson()));
        }

        [TestMethod]
        public void ResourceRequestManager_ParseTokenParameters_WithEmptyResourceType_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentException>(() => m.ParseTokenParameters(string.Empty, this.GenerateTableJson()));
        }

        [TestMethod]
        public void ResourceRequestManager_ParseTokenParameters_WithWhitespaceResourceType_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentException>(() => m.ParseTokenParameters(" ", this.GenerateTableJson()));
        }

        [TestMethod]
        public void ResourceRequestManager_ParseTokenParameters_WithNullJsonParameters_ThrowsArgumentException()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentException>(() => m.ParseTokenParameters("table", (JToken)null));
        }

        [TestMethod]
        public void ResourceRequestManager_ParseTokenParameters_WithNullJsonParameters_ThrowsArgumentException2()
        {
            ResourceRequestManager m = new ResourceRequestManager();
            this.ExpectException<ArgumentException>(() => m.ParseTokenParameters(ResourceType.Table, (JToken)null));
        }

        [TestMethod]
        public void ResourceRequestManager_ParseTokenParameters_WithMissingName_ThrowsException()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            JToken parameters = this.GenerateTableJson();
            parameters["name"] = string.Empty;

            // Act
            this.ExpectException<HttpResponseException>(() => m.ParseTokenParameters("table", parameters));
        }

        [TestMethod]
        public void ResourceRequestManager_ParseTokenParameters_WithMissingPermissions_ThrowsException()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            JToken parameters = this.GenerateTableJson();
            parameters["permissions"] = string.Empty;

            // Act
            this.ExpectException<HttpResponseException>(() => m.ParseTokenParameters("table", parameters));
        }

        [TestMethod]
        public void ResourceRequestManager_ParseTokenParameters_WithInvalidPermissions_ThrowsException()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            JToken parameters = this.GenerateTableJson();
            parameters["permissions"] = "asdfadsf";

            // Act
            this.ExpectException<HttpResponseException>(() => m.ParseTokenParameters("table", parameters));
        }

        [TestMethod]
        public void ResourceRequestManager_ParseTokenParameters_WithMissingDate_ThrowsException()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            JToken parameters = this.GenerateTableJson();
            parameters["expiry"] = string.Empty;

            // Act
            this.ExpectException<HttpResponseException>(() => m.ParseTokenParameters("table", parameters));
        }

        [TestMethod]
        public void ResourceRequestManager_ParseTokenParameters_WithInvalidDate_ThrowsException()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();
            JToken parameters = this.GenerateTableJson();
            parameters["expiry"] = "asfasdf";

            // Act
            this.ExpectException<HttpResponseException>(() => m.ParseTokenParameters("table", parameters));
        }

        [TestMethod]
        public void ResourceRequestManager_ParseTokenParameters_ReturnsResourceParameters()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();

            // Act
            ResourceParameters parameters = m.ParseTokenParameters("table", this.GenerateTableJson());

            // Assert
            Assert.AreEqual("table", parameters.Name);
            Assert.AreEqual(ResourcePermissions.ReadWrite, parameters.Permissions);
            Assert.IsTrue(parameters.Expiration > DateTime.UtcNow - TimeSpan.FromSeconds(1) && parameters.Expiration < DateTime.UtcNow + TimeSpan.FromSeconds(1));
        }

        [TestMethod]
        public void ResourceRequestManager_ParseTokenParameters_ReturnsResourceParameters2()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();

            // Act
            ResourceParameters parameters = m.ParseTokenParameters(ResourceType.Table, this.GenerateTableJson());

            // Assert
            Assert.AreEqual("table", parameters.Name);
            Assert.AreEqual(ResourcePermissions.ReadWrite, parameters.Permissions);
            Assert.IsTrue(parameters.Expiration > DateTime.UtcNow - TimeSpan.FromSeconds(1) && parameters.Expiration < DateTime.UtcNow + TimeSpan.FromSeconds(1));
        }

        [TestMethod]
        public void ResourceRequestManager_ParseTokenParameters_ReturnsBlobToken()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();

            // Act
            BlobParameters parameters = (BlobParameters)m.ParseTokenParameters("blob", this.GenerateBlobJson());

            // Assert
            Assert.AreEqual("container", parameters.Container);
            Assert.AreEqual("blob", parameters.Name);
            Assert.AreEqual(ResourcePermissions.ReadWrite, parameters.Permissions);
            Assert.IsTrue(parameters.Expiration > DateTime.UtcNow - TimeSpan.FromSeconds(1) && parameters.Expiration < DateTime.UtcNow + TimeSpan.FromSeconds(1));
        }

        [TestMethod]
        public void ResourceRequestManager_ParseTokenParameters_ReturnsBlobToken2()
        {
            // Setup
            ResourceRequestManager m = new ResourceRequestManager();

            // Act
            BlobParameters parameters = (BlobParameters)m.ParseTokenParameters(ResourceType.Blob, this.GenerateBlobJson());

            // Assert
            Assert.AreEqual("container", parameters.Container);
            Assert.AreEqual("blob", parameters.Name);
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

        private JToken GenerateBlobJson()
        {
            JToken token = this.GenerateTableJson();
            token["name"] = "blob";
            token["container"] = "container";
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