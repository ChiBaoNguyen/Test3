using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebAPI.Handlers;

namespace WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            log4net.Config.XmlConfigurator.Configure();
            // Phat Nguyen - 14 / 08 / 2014
            //at some point, experienced the problem where their API is returning a 500 Internal Server Error, 
            //but tracing through with Visual Studio doesn't reveal any exceptions in our code.  
            //This problem is often caused when a MediaTypeFormatter is unable to serialize an object.  
            //This simple message handler can take away some of the pain of debugging these scenarios.
            config.MessageHandlers.Add(new BufferNonStreamedContentHandler());
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
