using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Website.ViewModels.Feature;

namespace WebAPI.Providers
{
	public class ExtendedRolesProvider
	{
		private readonly RoleFeaturesRepository _roleFeaturesRepository;
		private readonly FeaturesRepository _featuresRepository;
		private readonly BasicRepository _basicRepository;
		private readonly EmployeeRepository _employeeRepository;
		private readonly DriverRepository _driverRepository;

		public ExtendedRolesProvider()
		{
			_roleFeaturesRepository = new RoleFeaturesRepository(new DatabaseFactory());
			_featuresRepository = new FeaturesRepository(new DatabaseFactory());
			_basicRepository = new BasicRepository(new DatabaseFactory());
			_employeeRepository = new EmployeeRepository(new DatabaseFactory());
			_driverRepository = new DriverRepository(new DatabaseFactory());
		}

		public IEnumerable<Claim> AddRoles(string roleId)
		{
			var claims = new List<Claim>();
			var roleFeatures = GetUserRoleFeature(roleId);
			if (!roleFeatures.Any()) return claims;
			claims.AddRange(roleFeatures.Select(t => new Claim(ClaimTypes.Role, t.FeatureC)));

			return claims;
		}

		public List<FeatureViewModel> GetUserRoleFeature(string roleId)
		{
			var features = _featuresRepository.GetAllQueryable();
			var unusedFeatures = (from p in _roleFeaturesRepository.GetAllQueryable()
								where p.RoleId == roleId
								select p.UnusedFeatureC).ToList();

			var usedFeatures = (from p in features
								where !unusedFeatures.Contains(p.FeatureC)
								select new FeatureViewModel()
								{
									FeatureC = p.FeatureC,
									FeatureN = p.FeatureN
								}).ToList();

			return usedFeatures;
		}

		public int GetAccessTokenTimeout()
		{
			var accessTokenTimeout = 30;
			var basicSetting = _basicRepository.GetAll().ToList();
			if (basicSetting.Any())
			{
				accessTokenTimeout = basicSetting[0].AccessTokenTimeout;
			}
			return accessTokenTimeout;
		}

		public EmployeeInfo GetEmployeeInfo(string empC)
		{
			var emp = new EmployeeInfo();
			var employee = _employeeRepository.Query(p => p.EmployeeC == empC).FirstOrDefault();
			if (employee != null)
			{
				emp.EmployeeC = employee.EmployeeC;
				emp.EmployeeN = employee.EmployeeLastN + " " + employee.EmployeeFirstN;
				return emp;
			}
			return null;
		}

		public DriverInfo GetDriverInfo(string drvC)
		{
			var drv = new DriverInfo();
			var driver = _driverRepository.Query(p => p.DriverC == drvC).FirstOrDefault();
			if (driver != null)
			{
				drv.DriverC = driver.DriverC;
				drv.DriverN = driver.LastN + " " + driver.FirstN;
				return drv;
			}
			return null;
		}
	}

	public class EmployeeInfo
	{
		public string EmployeeC { get; set; }
		public string EmployeeN { get; set; }
		public string IsActive { get; set; }
	}

	public class DriverInfo
	{
		public string DriverC { get; set; }
		public string DriverN { get; set; }
		public string IsActive { get; set; }
	}
}