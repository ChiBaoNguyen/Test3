using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Root.Data.Infrastructure;
using Root.Data.Repository;

namespace WebAPI.Filters
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public class AuthorizeAttribute : System.Web.Http.AuthorizeAttribute
	{
		protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
		{
			if (actionContext.RequestContext.Principal.Identity.IsAuthenticated)
			{
				actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
			}
			else
			{
				IEnumerable<string> values;
				if (actionContext.Request.Headers.TryGetValues("UserSSTP", out values))
				{
					var userSstp = values.First().Split('|');
					var sstp = userSstp[0];
					var userName = userSstp[1];
					if (!string.IsNullOrEmpty(sstp))
					{
						var databaseFactory = new DatabaseFactory();
						var userRepository = new UserRepository(databaseFactory);
						var isValidSstp = userRepository.CheckSecurityStamp(userName, sstp);
						if (isValidSstp)
						{
							var user = userRepository.Query(p => p.UserName.Equals(userName)).FirstOrDefault();
							if (user != null && user.IsLoggedIn == "1")
							{
								user.IsLoggedIn = "0";
								userRepository.Update(user);
								databaseFactory.Get().SaveChanges();
							}
						}
					}
				}
				base.HandleUnauthorizedRequest(actionContext);
			}
		}
	}
}