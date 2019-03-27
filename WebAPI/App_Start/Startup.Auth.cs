
using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac.Integration.WebApi;
//using InteractivePreGeneratedViews;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using Root.Data;
using Root.Migrations;
using WebAPI.App_Start;
using WebAPI.Handlers;
using WebAPI.Providers;
using System.Data.Entity;

namespace WebAPI
{
	public partial class Startup
	{
		static Startup()
		{
			//PublicClientId = "self";

			//UserManagerFactory = () => new UserManager<IdentityUser>(new UserStore<IdentityUser>());

			//OAuthOptions = new OAuthAuthorizationServerOptions
			//{
			//	TokenEndpointPath = new PathString("/Token"),
			//	Provider = new ApplicationOAuthProvider(PublicClientId, UserManagerFactory),
			//	AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
			//	AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
			//	AllowInsecureHttp = true
			//};
		}

		public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

		//public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

		//public static Func<UserManager<IdentityUser>> UserManagerFactory { get; set; }

		//public static string PublicClientId { get; private set; }

		// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
		{
			#region Generate view for improving performance and Migration DB
			////Generate view in file
			//using (var ctx = new SGTSVNDBContext())
			//{
			//	InteractiveViews
			//		.SetViewCacheFactory(
			//			ctx,
			//			new FileViewCacheFactory(System.Web.Hosting.HostingEnvironment.MapPath("~/DBContextViews.xml")));
			//}

			//Generate view store in DB
			//using (var ctx = new SGTSVNDBContext())
			//{
			//	InteractiveViews
			//		.SetViewCacheFactory(
			//			ctx,
			//			new SqlServerViewCacheFactory(ctx.Database.Connection.ConnectionString));
			//}

			////Migration DB
			//giam 2 giay
			//Database.SetInitializer<SGTSVNDBContext>(null);
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<SGTSVNDBContext, Configuration>());
			#endregion
			

			// Branch the pipeline here for requests that start with "/signalr"
			app.Map("/signalr", map =>
			{
				// Setup the CORS middleware to run before SignalR.
				// By default this will allow all origins. You can 
				// configure the set of origins and/or http verbs by
				// providing a cors options with a different policy.
				map.UseCors(CorsOptions.AllowAll);
				var hubConfiguration = new HubConfiguration
				{
					// You can enable JSONP by uncommenting line below.
					// JSONP requests are insecure but some older browsers (and some
					// versions of IE) require JSONP to work cross domain
					// EnableJSONP = true
				};
				// Run the SignalR pipeline. We're not using MapSignalR
				// since this branch already runs under the "/signalr"
				// path.
				map.RunSignalR(hubConfiguration);
			});

			var container = Bootstrapper.SetAutofacContainer();
			var config = new HttpConfiguration();
			//WebApiConfig.Register(config);
			//config.Formatters.Clear();
			//config.Formatters.Add(new JsonMediaTypeFormatter());

			//config.Formatters.JsonFormatter.SerializerSettings =
			//	new JsonSerializerSettings
			//	{
			//		ContractResolver = new CamelCasePropertyNamesContractResolver()
			//	};
			//config.MessageHandlers.Add(new BufferNonStreamedContentHandler());

			// Web API routes
			config.MapHttpAttributeRoutes();
			log4net.Config.XmlConfigurator.Configure();
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);

			ConfigureOAuth(app);

			config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

			app.UseAutofacMiddleware(container);
			app.UseAutofacWebApi(config);

			app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
			app.UseWebApi(config);

			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			Bootstrapper.Run();
		}

		public void ConfigureOAuth(IAppBuilder app)
		{
			//use a cookie to temporarily store information about a user logging in with a third party login provider
			app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);
			OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

			var OAuthServerOptions = new OAuthAuthorizationServerOptions()
			{
				AllowInsecureHttp = true,
				TokenEndpointPath = new PathString("/token"),
				AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
				Provider = new SimpleAuthorizationServerProvider(),
				RefreshTokenProvider = new SimpleRefreshTokenProvider()
			};

			// Token Generation
			app.UseOAuthAuthorizationServer(OAuthServerOptions);
			app.UseOAuthBearerAuthentication(OAuthBearerOptions);

		}
	}
}
