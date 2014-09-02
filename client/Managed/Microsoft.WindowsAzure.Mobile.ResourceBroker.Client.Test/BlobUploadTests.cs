using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.MobileServices;

namespace Microsoft.WindowsAzure.Mobile.ResourceBroker.Client.Test
{
    [TestClass]
    public class BlobUploadTests
    {
        const string AppUrl = "https://SERVICE-NAME.azure-mobile.net/";
        const string AppKey = "APPLICATION-KEY";
        const string ContainerName = "containername";

        static readonly string testBlobContent = string.Format("Content for the blob on {0}",
            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));

        [TestCleanup]
        public void TestCleanup()
        {
            ResourceBrokerClientExtensions.HttpClientCreator = () => new HttpClient();
        }

        [TestMethod]
        public async Task UploadBlob_UrlReturnedFromBroker_IsUsedToUploadToStorage()
        {
            var mobileServiceHandler = new TestHttpHandler(HttpStatusCode.OK);
            var sasToken = "http://the.sas.token.com/url?query=string";
            var sasTokenNoQuery = "http://the.sas.token.com/url";
            mobileServiceHandler.SetResponse("{\"uri\":\"" + sasToken + "\"}", "application/json");
            var client = new MobileServiceClient(
                AppUrl,
                AppKey,
                mobileServiceHandler);

            var storageHandler = new TestHttpHandler(HttpStatusCode.Created);
            ResourceBrokerClientExtensions.HttpClientCreator = () => new HttpClient(storageHandler);

            var ms = GetBlobContent();
            var url = await client.UploadFileToBlobStorage("container", "file", "text/plain", ms);

            Assert.AreEqual(sasTokenNoQuery, url.ToString());
            Assert.AreEqual(sasToken, storageHandler.RequestUrl);
            Assert.AreEqual(testBlobContent, storageHandler.RequestContent);
        }

        [TestMethod]
        public async Task UploadBlob_ServiceReturns404_ExpectedExceptionIsCaught()
        {
            var mobileServiceHandler = new TestHttpHandler(HttpStatusCode.NotFound);
            var client = new MobileServiceClient(
                AppUrl,
                AppKey,
                mobileServiceHandler);

            var storageHandler = new TestHttpHandler(HttpStatusCode.Created);
            ResourceBrokerClientExtensions.HttpClientCreator = () => new HttpClient(storageHandler);

            var ms = GetBlobContent();
            try
            {
                var url = await client.UploadFileToBlobStorage("container", "file", "text/plain", ms);
                Assert.Fail("Upload call should have thrown an exception");
            }
            catch (InvalidOperationException e)
            {
                Assert.IsNotNull(e.InnerException);
                Assert.IsInstanceOfType(e.InnerException, typeof(MobileServiceInvalidOperationException));
                Assert.AreEqual(HttpStatusCode.NotFound, ((MobileServiceInvalidOperationException)e.InnerException).Response.StatusCode);
            }
        }

        [TestMethod]
        public async Task UploadBlob_BrokerReturnsInvalidResponse_ExpectedExceptionIsCaught()
        {
            var mobileServiceHandler = new TestHttpHandler(HttpStatusCode.OK);
            mobileServiceHandler.SetResponse("{\"other\":\"value\"}", "application/json");
            var client = new MobileServiceClient(
                AppUrl,
                AppKey,
                mobileServiceHandler);

            var storageHandler = new TestHttpHandler(HttpStatusCode.Created);
            ResourceBrokerClientExtensions.HttpClientCreator = () => new HttpClient(storageHandler);

            var ms = GetBlobContent();
            try
            {
                var url = await client.UploadFileToBlobStorage("container", "file", "text/plain", ms);
                Assert.Fail("Upload call should have thrown an exception");
            }
            catch (InvalidOperationException) { }
        }

        [TestMethod]
        public async Task UploadBlob_StorageReturnsError_AppropriateExceptionIsThrown()
        {
            var mobileServiceHandler = new TestHttpHandler(HttpStatusCode.OK);
            var sasToken = "http://the.sas.token.com/url?query=string";
            var sasTokenNoQuery = "http://the.sas.token.com/url";
            mobileServiceHandler.SetResponse("{\"uri\":\"" + sasToken + "\"}", "application/json");
            var client = new MobileServiceClient(
                AppUrl,
                AppKey,
                mobileServiceHandler);

            var storageHandler = new TestHttpHandler(HttpStatusCode.InternalServerError);
            storageHandler.SetResponse("An error occurred");
            ResourceBrokerClientExtensions.HttpClientCreator = () => new HttpClient(storageHandler);

            var ms = GetBlobContent();
            try
            {
                var url = await client.UploadFileToBlobStorage("container", "file", "text/plain", ms);
            }
            catch (InvalidOperationException e)
            {
                Assert.IsNotNull(e.InnerException);
                Assert.IsInstanceOfType(e.InnerException, typeof(HttpRequestException));
            }
        }

        private MemoryStream GetBlobContent()
        {
            return new MemoryStream(new UTF8Encoding(false).GetBytes(testBlobContent));
        }
    }
}
