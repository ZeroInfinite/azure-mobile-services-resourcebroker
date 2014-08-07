Azure Mobile Services ResourceBroker Extension
====================================

The Mobile Services ResourceBroker extension makes it easy to serve up access constrained resource tokens for other Azure services, and to consume those from your mobile clients. Common examples are providing an API to upload a file to a blob container, or to give a user access to read from a specific Azure table.

The ResourceBroker consists of several components:

* Back-end extensions for Mobile Services .NET or Node.
* Front-end extensions for managed, iOS, and Android clients (client support coming soon!)

# Back-End #

## .NET ##
To expose a ResourceBroker controller from your .NET Mobile Services back-end, perform the following steps:

### Storage ###

Create an Azure Storage account, and take note of the primary access key.

### Portal ###

Navigate to your Mobile Service in the Azure portal, and flip to the Configure tab. Add an app setting named *ResourceBrokerStorageConnectionString*, and set the value as your primary storage access key. If you wish to use different storage accounts for blob and table storage, you can alternately use the *ResourceBrokerBlobConnectionString* and *ResourceBrokerTableConnectionString* app settings.

### Code ###

First, import the ResourceBroker Nuget package into your Mobile Services back-end project. Next, create a new controller and derive it from the ResourceBroker class:
    
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class MyResourcesController : ResourcesController
    {
    }

### Done! ###

Now your Mobile Services back-end is ready to issue signed, scoped access tokens for blob and table storage. 

To learn how to execute your API directly, see the REST API section under Client, below.

### Advanced ###

Rather than using the default controller, you may wish to do further customization. There are several ways to do this. First, you can override the *GenerateToken* method:

    protected override ResourceToken GenerateToken(string type, JToken parameters)
    {
        // Perform custom authorization here...

        return base.GenerateToken(type, parameters);
    }
  
This allows you to perform request validation before issuing the token. Alternately, you can take over the entire flow by providing a custom controller:

    [AuthorizeLevel(AuthorizationLevel.User)]
    public abstract class ResourcesController : ApiController
    {
        private ResourceRequestManager requestManager = new ResourceRequestManager();

        public ResourceToken Post(string type, [FromBody] JToken parameters)
        {
            return this.requestManager.GenerateToken(type, parameters, this.Services.Settings);
        }

        /// <summary>
        /// The services property.
        /// </summary>
        public ApiServices Services
        {
            get;
            set;
        }
    }


# Client #

## Rest API ##

This section describes the details of the ResourceBroker API, so that you can issue direct REST calls to the API.

### URI ###

All requests to the ResourceBroker endpoint must take the form of a POST request. To request a token for Azure Table Storage, use:

    https://{my mobile service}/api/resources/table

Similarly, to request a blob token, use:

    https://{my mobile service}/api/resources/blob

### Body ###

The body of the POST request must contain a JSON payload containing the detail of the request. This is where you specify the table or blob name, access level, expiration time, and other parameters.

**name**: The name of the blob or table to access

**permissions**: The requested access permissions. The valid values are r, w, or rw, indicating read-only access, write-only access, or read-write access

**expiry**: The requested expiration date and time for the resource token

**container**: The name of the blob container. Note that the container must already exist, or the request will fail. This value is not required for table storage requests.

#### Example ####

The following example shows the body payload requesting an access token for a blob named 'myblob', within the container 'mycontainer', with write-only access, expiring on December 12, 2015.

    {
	    'name': 'myblob',
	    'container': mycontainer',
	    'permissions': 'w'
        'expiry': '2015-12-01T07:34:42-5:00'
	}