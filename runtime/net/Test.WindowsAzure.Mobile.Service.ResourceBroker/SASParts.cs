using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.WindowsAzure.Mobile.Service.ResourceBroker
{
    public class SASParts
    {
        public SASParts(string uri)
        {
            this.QueryParameters = new Dictionary<string, string>();

            string[] parts = uri.Trim().Split('?');
            if (parts.Length > 0)
            {
                this.HostName = parts[0].Trim();
            }

            if (parts.Length > 1)
            {
                string[] parameters = parts[1].Trim().Split('&');
                foreach (string parameter in parameters)
                {
                    string[] keyValue = parameter.Trim().Split('=');
                    if (keyValue.Length == 1)
                    {
                        this.QueryParameters.Add(keyValue[0].Trim(), string.Empty);
                    }
                    else if (keyValue.Length > 1)
                    {
                        this.QueryParameters.Add(keyValue[0].Trim(), keyValue[1].Trim());
                    }
                }
            }
        }

        public string HostName
        {
            get;
            private set;
        }

        public Dictionary<string, string> QueryParameters
        {
            get;
            private set;
        }

        public string Value(string key)
        {
            string value = null;
            this.QueryParameters.TryGetValue(key, out value);
            return value;
        }
    }
}
