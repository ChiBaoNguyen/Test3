using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Service.Services;
using Root.Models.Authorization;
using Website.ViewModels.RoleFeatures;

namespace WebAPI.Controllers
{
	public class RoleFeaturesController : ApiController
	{
		public IRoleFeaturesService _roleFeaturesService;

		public RoleFeaturesController() { }
		public RoleFeaturesController(IRoleFeaturesService roleFeaturesService)
		{
			this._roleFeaturesService = roleFeaturesService;
		}

		[HttpGet]
		[Route("api/RoleFeatures/GetRoleFeatures")]
		public RoleFeaturesViewModel GetRoleFeatures(string roleId)
		{
			return _roleFeaturesService.GetRoleFeatures(roleId);
		}

		public void Post(RoleFeaturesViewModel data)
		{
			_roleFeaturesService.UpdateRoleFeatures(data);
		}

		public void Delete(string id)
		{
			_roleFeaturesService.DeleteRoleFeatures(id);
		}
	}
}