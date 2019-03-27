using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Root.AuthenticationModels;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using WebAPI.Handlers;

namespace WebAPI.Providers
{
	public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
	{
		public UserRepository _userRepository;

		public SimpleAuthorizationServerProvider()
		{
			_userRepository = new UserRepository(new DatabaseFactory());
		}

		public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
		{
			string clientId = string.Empty;
			string clientSecret = string.Empty;
			Client client = null;

			if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
			{
				context.TryGetFormCredentials(out clientId, out clientSecret);
			}

			if (context.ClientId == null)
			{
				//Remove the comments from the below line context.SetError, and invalidate context 
				//if you want to force sending clientId/secrects once obtain access tokens. 
				context.Validated();
				context.OwinContext.Set<string>("as:IsMobile", context.Parameters["isMobile"]);
				return Task.FromResult<object>(null);
			}

			client = _userRepository.FindClient(context.ClientId);

			if (client == null)
			{
				context.SetError("invalid_clientId", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
				return Task.FromResult<object>(null);
			}

			if (client.ApplicationType == ApplicationTypes.NativeConfidential)
			{
				if (string.IsNullOrWhiteSpace(clientSecret))
				{
					context.SetError("invalid_clientId", "Client secret should be sent.");
					return Task.FromResult<object>(null);
				}
				else
				{
					if (client.Secret != Helper.GetHash(clientSecret))
					{
						context.SetError("invalid_clientId", "Client secret is invalid.");
						return Task.FromResult<object>(null);
					}
				}
			}

			if (!client.Active)
			{
				context.SetError("invalid_clientId", "Client is inactive.");
				return Task.FromResult<object>(null);
			}

			context.OwinContext.Set<string>("as:clientAllowedOrigin", client.AllowedOrigin);
			context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

			context.Validated();
			return Task.FromResult<object>(null);
		}

		public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{

			var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
			var isMobile = context.OwinContext.Get<string>("as:IsMobile") == "1";

			if (allowedOrigin == null) allowedOrigin = "*";

			context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

			User user = await _userRepository.FindUser(context.UserName, context.Password);

			if (user == null)
			{
				context.SetError("invalid_grant", "WRONGINFO");
				return;
			}

			if ((isMobile & string.IsNullOrEmpty(user.DriverC)) ||
					(!isMobile & !string.IsNullOrEmpty(user.DriverC)))
			{
				context.SetError("invalid_grant", "NOPERMISSION");
				return;
			}

			if (user.IsActive == "0")
			{
				context.SetError("invalid_grant", "DEACTIVED");
				return;
			}

			var identity = new ClaimsIdentity(context.Options.AuthenticationType);
			identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
			var roleId = user.Roles.ToList()[0].RoleId;
			var roleFeatureProvider = new ExtendedRolesProvider();
			identity.AddClaims(roleFeatureProvider.AddRoles(roleId));

			//get user role features
			var features = from p in roleFeatureProvider.GetUserRoleFeature(roleId)
							select p.FeatureC;
			var jwtFeatures = "";
			var builder = new StringBuilder();
			builder.Append("[");
			foreach (var featureC in features)
			{
				builder.Append("\"" + featureC + "\", ");
			}
			builder.Append("]");
			jwtFeatures = builder.ToString();

			var employeeInfo = roleFeatureProvider.GetEmployeeInfo(user.EmployeeC);
			var employeeC = employeeInfo != null ? user.EmployeeC : string.Empty;
			var employeeN = employeeInfo != null ? employeeInfo.EmployeeN : string.Empty;

			var driverInfo = roleFeatureProvider.GetDriverInfo(user.DriverC);
			var driverC = driverInfo != null ? user.DriverC : string.Empty;
			var driverN = driverInfo != null ? driverInfo.DriverN : string.Empty;
			var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    { 
                        "as:client_id", context.ClientId ?? string.Empty
                    },
                    { 
                        "userName", context.UserName
                    },
					{ 
						"empC", employeeC
					},
					{ 
						"empN", employeeN
					},
					{
						"drvC", driverC
					},
					{
						"drvN", driverN
					},
					{
		                "sstp", user.SecurityStamp + "|" + context.UserName
	                },
	                {
		                "permissions", jwtFeatures
	                }
                });

			var ticket = new AuthenticationTicket(identity, props);
			var accessTokenTimeout = roleFeatureProvider.GetAccessTokenTimeout();
			context.Options.AccessTokenExpireTimeSpan = isMobile ? TimeSpan.FromDays(1000) : TimeSpan.FromMinutes(accessTokenTimeout);
			context.Validated(ticket);
		}

		public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
		{
			var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
			var currentClient = context.ClientId;

			if (originalClient != currentClient)
			{
				context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
				return Task.FromResult<object>(null);
			}

			// Change auth ticket for refresh token requests
			var newIdentity = new ClaimsIdentity(context.Ticket.Identity);

			var newClaim = newIdentity.Claims.FirstOrDefault(c => c.Type == "newClaim");
			if (newClaim != null)
			{
				newIdentity.RemoveClaim(newClaim);
			}
			newIdentity.AddClaim(new Claim("newClaim", "newValue"));

			var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
			context.Validated(newTicket);

			return Task.FromResult<object>(null);
		}

		public override Task TokenEndpoint(OAuthTokenEndpointContext context)
		{
			foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
			{
				context.AdditionalResponseParameters.Add(property.Key, property.Value);
			}

			return Task.FromResult<object>(null);
		}

	}
}