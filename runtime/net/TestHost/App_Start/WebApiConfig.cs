using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;

namespace TestHost
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            // Insert the ResourcesController route at the top to avoid conflicting with predefined routes.
            var resourcesRoute = config.Routes.CreateRoute(
                routeTemplate: "api/resources/{type}",
                defaults: new { controller = "resources" },
                constraints: null);

            config.Routes.Insert(0, "Resources", resourcesRoute);

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
        }
    }
}