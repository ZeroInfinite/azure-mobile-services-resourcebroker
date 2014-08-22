using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Brokers;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Newtonsoft.Json.Linq;

namespace TestHost
{
    /// <summary>
    /// Issues tokens and connection strings for various Azure resources.
    /// </summary>
    /// <remarks>Set the permission level to User if you want to restrict access to only authenticated users.</remarks>
    [AuthorizeLevel(AuthorizationLevel.Anonymous)]
    public class ResourcesController : ResourcesControllerBase
    {
    }
}