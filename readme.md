Azure Mobile Services ResourceBroker Extension
====================================

The Mobile Services ResourceBroker extension makes it easy to serve up access constrained resource tokens for other Azure services, and to consume those from your mobile clients. Common examples are providing an API to upload a file to a blob container, or to give a user access to read from a specific Azure table.

The ResourceBroker consists of several components:

* Back-end extensions for Mobile Services .NET or Node.
* Front-end extensions for managed, iOS, and Android clients (client support coming soon!)

# Back-End #
## Node.js Back-End ##
To expose a ResourceBroker api from your node Mobile Services back-end, perform the following steps:

### Installation ###
Clone your Mobile Service repository and install the npm module in the 'service' folder

    $ npm install mobileservice-resourcebroker
    
### Get Started ###

Create a new custom api on your Mobile Service called resources from azure cli like so:
```js
azure mobile api create yourservice resources
```
In your Mobile Service repository update the resources.js script with the snippet as follows:
```js
exports.register = function (api) {
	var config = require('mobileservice-config');
	var options = { config: config.appSettings };
    require('mobileservice-resourcebroker').Resources.register(api, options);
};
```
In your Mobile Service configuration settings you need to set the following settings:

#### Blob storage ####
* _ResourceBrokerBlobStoragePermissions_: This sets the permissions that client can ask for blob storage. Possible values are 'r', 'rw', or 'w' for read, read-write and write respectively.
* _ResourceBrokerBlobStorageAccountName_: The name of storage account to use for blob storage.
* _ResourceBrokerBlobStorageAccountKey_: The key of storage account to use for blob storage.

#### Table storage ####
* _ResourceBrokerTableStoragePermissions_: This sets the permissions that client can ask for table storage. Possible values are 'r', 'rw' or 'w' for read, read-write and write respectively.
* _ResourceBrokerTableStorageAccountName_: The name of storage account to use for table storage.
* _ResourceBrokerTableStorageAccountKey_: The key of storage account to use for table storage.

Note: If you want to share same storage account for both blob and table, you can use ResourceBrokerStorageAccountName and ResourceBrokerStorageAccountKey settings instead.

## .NET Back-End ##
To expose a ResourceBroker controller from your .NET Mobile Services back-end, perform the following steps:

### Installation ###
Clone your Mobile Service repository and install the Nuget module to your Mobile Services back-end project. Enter the following into the Package Manager Console in Visual Studio:

    Install-Package Microsoft.WindowsAzure.Mobile.Service.ResourceBroker 

### Get Started ###

Create an Azure Storage account, and take note of the primary access key.

### Portal ###

Navigate to your Mobile Service in the Azure portal, and flip to the Configure tab. Add an app setting named *ResourceBrokerStorageConnectionString*, and set the value as your primary storage access key. If you wish to use different storage accounts for blob and table storage, you can alternately use the *ResourceBrokerBlobConnectionString* and *ResourceBrokerTableConnectionString* app settings.

### Code ###

First, import the ResourceBroker Nuget package into your Mobile Services back-end project. Next, create a new controller and derive it from the ResourceBrokerBase class:
    
    // Setting the authorization level to User ensures that only authenticated users can request resource tokens.
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class ResourcesController : ResourcesControllerBase
    {
    }

Finally, add a custom route to map to this controller. Insert this code within the Register method of your WebApiConfig file:

    // Create a custom route mapping the resource type into the URI.     
    var resourcesRoute = config.Routes.CreateRoute(
         routeTemplate: "api/resources/{type}",
         defaults: new { controller = "resources" },
         constraints: null);

     // Insert the ResourcesController route at the top of the collection to avoid conflicting with predefined routes. 
     config.Routes.Insert(0, "Resources", resourcesRoute);

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

**permissions**: The requested access permissions. The possible values are: 'raudp', for read, add, update, delete, and process, respectively. Individual values may be omitted if desired. For example, a permission request containing 'ud' would give access to update and delete items, but not to read, add, or process items. It is also possible to specify the 'w' value as a shortcut for write access. 'w' is equivalent to 'aud'. For example, the 'rw' permission string would give access to all reads and writes, though it does not include the process permission.

**expiry**: The requested expiration date and time for the resource token

**container**: The name of the blob container. Note that the container must already exist, or the request will fail. This value is not required for table storage requests.

#### Example ####

The following example shows the body payload requesting an access token for a blob named "myblob", within the container "mycontainer", with write-only access, expiring on December 1, 2015.

    {
	    "name": "myblob",
	    "container": "mycontainer",
	    "permissions": "w",
        "expiry": "2015-12-01T07:34:42Z"
	}

The following example shows the body payload requesting a token for a queue named "myqueue", with read, add, update, and process permissions:

    {
	    "name": "myqueue",
	    "permissions": "raup",
        "expiry": "2015-12-01T07:34:42Z"
	}


# Need Help? #
Be sure to check out the Mobile Services [Developer Forum](http://social.msdn.microsoft.com/Forums/en-US/azuremobile/) if you are having trouble. The Mobile Services product team actively monitors the forum and will be more than happy to assist you.

# Contribute Code or Provide Feedback #
If you would like to become an active contributor to this project please follow the instructions provided in [Microsoft Azure Projects Contribution Guidelines](http://azure.github.com/guidelines.html).

If you encounter any bugs with the library please file an issue in the Issues section of the project.

# Learn More #
[Microsoft Azure Mobile Services Developer Center](http://azure.microsoft.com/en-us/develop/mobile)

