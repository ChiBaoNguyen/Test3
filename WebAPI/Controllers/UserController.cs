using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Root.AuthenticationModels;
using Root.Data.Repository;
using Service.Services;
using Website.ViewModels.User;

namespace WebAPI.Controllers
{
	public class UserController : ApiController
	{
		public IUserService _userService;
		public IUserRepository _userRepository;
		private IAuthenticationManager Authentication
		{
			get { return Request.GetOwinContext().Authentication; }
		}
		public UserController() { }
		public UserController(IUserService userService, IUserRepository userRepository)
		{
			this._userRepository = userRepository;
			this._userService = userService;
		}
		// POST api/Account/Register

		[Filters.Authorize(Roles = "UserRegistration_M_A")]
		[Route("api/User/Register")]
		public async Task<IHttpActionResult> Register(UserModel userModel)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			userModel.IsMobileUser = "0";
			IdentityResult result = await _userRepository.RegisterUser(userModel);

			IHttpActionResult errorResult = GetErrorResult(result);

			if (errorResult != null)
			{
				return errorResult;
			}

			return Ok();
		}

		[Filters.Authorize(Roles = "MobileUserRegistration_M_A")]
		[Route("api/User/MobileRegister")]
		public async Task<IHttpActionResult> MobileRegister(UserModel userModel)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			userModel.IsMobileUser = "1";
			IdentityResult result = await _userRepository.RegisterUser(userModel);

			IHttpActionResult errorResult = GetErrorResult(result);

			if (errorResult != null)
			{
				return errorResult;
			}

			return Ok();
		}

		[Filters.Authorize(Roles = "UserRegistration_M_E, MobileUserRegistration_M_E")]
		[Route("api/User/Update")]
		public async Task<IHttpActionResult> Update(UserViewModel userViewModel)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			UserModel user = new UserModel();
			user.UserName = userViewModel.UserName;
			user.RoleName = userViewModel.RoleN;
			user.EmployeeC = userViewModel.EmployeeC;
			user.DriverC = userViewModel.DriverC;
			user.IsActive = userViewModel.IsActive;

			IdentityResult result = await _userRepository.UpdateUser(user);

			IHttpActionResult errorResult = GetErrorResult(result);

			if (errorResult != null)
			{
				return errorResult;
			}

			return Ok();
		}

		[Filters.Authorize(Roles = "UserRegistration_M_D, MobileUserRegistration_M_D")]
		[Route("api/User/DoDeleteAccount")]
		public async Task<IHttpActionResult> DoDeleteAccount(UserViewModel userViewModel)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			UserModel user = new UserModel();
			user.UserName = userViewModel.UserName;
			user.RoleName = userViewModel.RoleN;

			IdentityResult result = await _userRepository.DeleteUser(user);

			IHttpActionResult errorResult = GetErrorResult(result);

			if (errorResult != null)
			{
				return errorResult;
			}

			return Ok();
		}

		[Filters.Authorize(Roles = "UserRegistration_M_E, MobileUserRegistration_M_E")]
		[Route("api/User/SetActiveStatus")]
		public async Task<IHttpActionResult> SetActiveStatus(UserViewModel userViewModel)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			UserModel user = new UserModel();
			user.UserName = userViewModel.UserName;
			user.IsActive = userViewModel.IsActive;

			IdentityResult result = await _userRepository.SetActiveStatus(user);

			IHttpActionResult errorResult = GetErrorResult(result);

			if (errorResult != null)
			{
				return errorResult;
			}

			return Ok();
		}

		[Filters.Authorize(Roles = "UserRegistration_M_E, MobileUserRegistration_M_E")]
		[HttpGet]
		[Route("api/User/ResetPassword/{userName}")]
		public async Task<IHttpActionResult> ResetPassword(string userName)
		{
			// set default password
			var defaultPassword = System.Configuration.ConfigurationManager.AppSettings["DefaultPassword"];

			IdentityResult result = await _userRepository.ResetPassword(userName, defaultPassword);

			IHttpActionResult errorResult = GetErrorResult(result);

			if (errorResult != null)
			{
				return errorResult;
			}

			return Ok();  
		}

		[HttpGet]
		[Route("api/User/ChangePassword/{userName}/{password}/{newPassword}")]
		public async Task<HttpResponseMessage> ChangePassword(string userName, string password, string newPassword)
		{
			IdentityResult result = await _userRepository.ChangePassword(userName, password, newPassword);

			string strResult = "OK";

			if (!result.Succeeded)
			{
				strResult = result.Errors.First();
			}

			return new HttpResponseMessage()
			{
				Content = new StringContent(
					strResult,
					Encoding.UTF8,
					"text/html"
				)
			};
		}
		private IHttpActionResult GetErrorResult(IdentityResult result)
		{
			if (result == null)
			{
				return InternalServerError();
			}

			if (!result.Succeeded)
			{
				if (result.Errors != null)
				{
					foreach (string error in result.Errors)
					{
						ModelState.AddModelError("", error);
					}
				}

				if (ModelState.IsValid)
				{
					// No ModelState errors are available to send, so just return an empty BadRequest.
					return BadRequest();
				}

				return BadRequest(ModelState);
			}

			return null;
		}

		private string ValidateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriOutput)
		{

			Uri redirectUri;

			var redirectUriString = GetQueryString(Request, "redirect_uri");

			if (string.IsNullOrWhiteSpace(redirectUriString))
			{
				return "redirect_uri is required";
			}

			bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

			if (!validUri)
			{
				return "redirect_uri is invalid";
			}

			var clientId = GetQueryString(Request, "client_id");

			if (string.IsNullOrWhiteSpace(clientId))
			{
				return "client_Id is required";
			}

			var client = _userRepository.FindClient(clientId);

			if (client == null)
			{
				return string.Format("Client_id '{0}' is not registered in the system.", clientId);
			}

			if (!string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
			{
				return string.Format("The given URL is not allowed by Client_id '{0}' configuration.", clientId);
			}

			redirectUriOutput = redirectUri.AbsoluteUri;

			return string.Empty;

		}

		private string GetQueryString(HttpRequestMessage request, string key)
		{
			var queryStrings = request.GetQueryNameValuePairs();

			if (queryStrings == null) return null;

			var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);

			if (string.IsNullOrEmpty(match.Value)) return null;

			return match.Value;
		}

		[Filters.Authorize(Roles = "UserRegistration_M, MobileUserRegistration_M")]
		[Route("api/User/Datatable")]
		public IHttpActionResult Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "UserName",
				  bool reverse = false,
				  string ul = "",
				  string search = null)
		{
			var custDatatable = _userService.GetUsersForTable(page, itemsPerPage, sortBy, reverse, ul, search);
			if (custDatatable == null)
			{
				return NotFound();
			}
			return Ok(custDatatable);
		}

		[Filters.Authorize(Roles = "UserRegistration_M, MobileUserRegistration_M")]
		[HttpGet]
		[Route("api/User/UserMobileDatatable")]
		public IHttpActionResult UserMobileDatatable(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "UserName",
				  bool reverse = false,
				  string search = null)
		{
			var custDatatable = _userService.GetMobileUsersForTable(page, itemsPerPage, sortBy, reverse, search);
			if (custDatatable == null)
			{
				return NotFound();
			}
			return Ok(custDatatable);
		}

		[HttpGet]
		[Route("api/User/GetUserByName")]
		public IHttpActionResult GetUserByName(string userName)
		{
			var user = _userService.GetUserByName(userName);
			return Ok(user);
		}

		[HttpGet]
		[Route("api/User/CheckUserLoggedIn")]
		public IHttpActionResult CheckUserLoggedIn(string userName, string password)
		{
			var status = _userService.CheckUserLoggedIn(userName, password);
			return Ok(status);
		}

		[HttpGet]
		[Route("api/User/SetUserLogOut")]
		public IHttpActionResult SetUserLogOut(string userName)
		{
			_userService.SetUserLogOut(userName);
			return Ok(true);
		}

		public IEnumerable<UserViewModel> Get(string value)
		{
			return _userService.GetUsers(value);
		}

		[HttpGet]
		[Route("api/User/GetMobileUsers")]
		public IEnumerable<UserViewModel> GetMobileUsers(string value)
		{
			return _userService.GetMobileUsers(value);
		}
        [HttpGet]
        [Route("api/User/GetDeviceInfo")]
        public IHttpActionResult GetDeviceInfo(string userName)
        {
            var device = _userService.GetDeviceInfo(userName);
            return Ok(device);
        }
        [HttpGet]
        [Route("api/User/SetDeviceInfo")]
        public IHttpActionResult SetDeviceInfo(string userName, string deviceInfo)
        {
            var updateDevice = _userService.SetDeviceInfo(userName, deviceInfo);
            return Ok(updateDevice);
        }
	}
}