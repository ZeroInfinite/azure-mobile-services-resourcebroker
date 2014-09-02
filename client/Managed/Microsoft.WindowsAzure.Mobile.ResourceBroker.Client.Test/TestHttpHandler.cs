using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzure.Mobile.ResourceBroker.Client.Test
{
    internal class TestHttpHandler : HttpMessageHandler
    {
        HttpStatusCode statusCode;
        string responseContent;
        string responseMediaType;

        public string RequestUrl { get; private set; }
        public string RequestContent { get; private set; }

        public TestHttpHandler(HttpStatusCode statusCode)
        {
            this.statusCode = statusCode;
        }

        public TestHttpHandler(HttpStatusCode statusCode, string textContent)
        {
            this.statusCode = statusCode;
            this.SetResponse(textContent, "text/plain");
        }

        public void SetResponse(string content, string mediaType = null)
        {
            if (mediaType == null)
            {
                mediaType = "text/plain";
            }

            this.responseContent = content;
            this.responseMediaType = mediaType;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = new HttpResponseMessage(this.statusCode);
            if (this.responseContent != null)
            {
                result.Content = new StringContent(this.responseContent, Encoding.UTF8, this.responseMediaType);
            }
            else
            {
                result.Content = new ByteArrayContent(new byte[0]);
            }

            this.RequestUrl = request.RequestUri.ToString();
            if (request.Content != null)
            {
                this.RequestContent = await request.Content.ReadAsStringAsync();
            }

            return result;
        }
    }
}
