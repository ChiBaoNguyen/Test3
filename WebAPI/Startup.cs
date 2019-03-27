using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using WebAPI.App_Start;

[assembly: OwinStartup(typeof(WebAPI.Startup))]

namespace WebAPI
{
    public partial class Startup
    {
		//public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public void Configuration(IAppBuilder app)
        {
			ConfigureAuth(app);
        }
    }
}
