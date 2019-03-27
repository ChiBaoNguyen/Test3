using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models.Authorization;
using Website.ViewModels.Role;
using Website.ViewModels.RoleFeatures;
using AutoMapper;
using Microsoft.AspNet.Identity.EntityFramework;
using Root.Models;

namespace Service.Services
{
	public interface IRoleFeaturesService
	{
		RoleFeaturesViewModel GetRoleFeatures(string roleId);
		void UpdateRoleFeatures(RoleFeaturesViewModel data);
		void DeleteRoleFeatures(string id);
	}
	public class RoleFeaturesService : IRoleFeaturesService
	{
		private readonly IRoleFeaturesRepository _roleFeaturesRepository;
		private readonly IRoleRepository _roleRepository;
		private readonly IUnitOfWork _unitOfWork;

		public RoleFeaturesService(IRoleFeaturesRepository roleFeaturesRepository,
								   IRoleRepository roleRepository,
								   IUnitOfWork unitOfWork)
		{
			this._roleFeaturesRepository = roleFeaturesRepository;
			this._roleRepository = roleRepository;
			this._unitOfWork = unitOfWork;
		}
		public void SaveRoleFeatures()
		{
			_unitOfWork.Commit();
		}

		public List<AuthorizedRoleFeature> GetAuthorizedRoleFeatures(string roleId)
		{
			var sRoleId = new SqlParameter("@roleId", roleId);
			var roleFeatures = _roleFeaturesRepository.ExecSpToGetRoleFeatures("GetUserRoleFeature @roleId", sRoleId);
			var listroleFeatures = roleFeatures.ToList();
			int count = listroleFeatures.Count;
			for (var i = 0; i < count; i++)
			{
				if (listroleFeatures[i].FeatureParentGroupN == "FTRADMINISTRATION" || listroleFeatures[i].FeatureParentGroupN == "FTRHOME" || listroleFeatures[i].FeatureGroupN == "FTRVESSELMASTER")
				{
					listroleFeatures.RemoveAt(i);
					i--;
					count--;
				}
			}
			int count2 = listroleFeatures.Count;
			for (var i = 0; i < count2; i++)
			{
				switch (i)
				{
					//3-4
					case 0:
						listroleFeatures[i].SortOrder = 3;
						break;
					case 1:
						listroleFeatures[i].SortOrder = 4;
						break;
					//5-9
					case 2:
						listroleFeatures[i].SortOrder = 8;
						break;
					case 3:
						listroleFeatures[i].SortOrder = 6;
						break;
					case 4:
						listroleFeatures[i].SortOrder = 7;
						break;
					case 5:
						listroleFeatures[i].SortOrder = 5;
						break;
					case 6:
						listroleFeatures[i].SortOrder = 9;
						break;
					//33-58
					case 7:
						listroleFeatures[i].SortOrder = 49;
						break;
					case 8:
						listroleFeatures[i].SortOrder = 48;
						break;
					case 9:
						listroleFeatures[i].SortOrder = 41;
						break;
					case 10:
						listroleFeatures[i].SortOrder = 39;
						break;
					case 11:
						listroleFeatures[i].SortOrder = 33;
						break;
					case 12:
						listroleFeatures[i].SortOrder = 44;
						break;
					case 13:
						listroleFeatures[i].SortOrder = 35;
						break;
					case 14:
						listroleFeatures[i].SortOrder = 55;
						break;
					case 15:
						listroleFeatures[i].SortOrder = 34;
						break;
					case 16:
						listroleFeatures[i].SortOrder = 50;
						break;
					case 17:
						listroleFeatures[i].SortOrder = 51;
						break;
					case 18:
						listroleFeatures[i].SortOrder = 52;
						break;
					case 19:
						listroleFeatures[i].SortOrder = 54;
						break;
					case 20:
						listroleFeatures[i].SortOrder = 53;
						break;
					case 21:
						listroleFeatures[i].SortOrder = 47;
						break;
					case 22:
						listroleFeatures[i].SortOrder = 36;
						break;
					case 23:
						listroleFeatures[i].SortOrder = 56;
						break;
					case 24:
						listroleFeatures[i].SortOrder = 40;
						break;
					case 25:
						listroleFeatures[i].SortOrder = 42;
						break;
					case 26:
						listroleFeatures[i].SortOrder = 46;
						break;
					case 27:
						listroleFeatures[i].SortOrder = 45;
						break;
					case 28:
						listroleFeatures[i].SortOrder = 43;
						break;
					case 29:
						listroleFeatures[i].SortOrder = 38;
						break;
					case 30:
						listroleFeatures[i].SortOrder = 37;
						break;
					case 31:
						listroleFeatures[i].SortOrder = 58;
						break;
					case 32:
						listroleFeatures[i].SortOrder = 57;
						break;
					//1-2
					case 33:
						listroleFeatures[i].SortOrder = 1;
						break;
					case 34:
						listroleFeatures[i].SortOrder = 2;
						break;
					//12-32
					case 35:
						listroleFeatures[i].SortOrder = 30;
						break;
					case 36:
						listroleFeatures[i].SortOrder = 32;
						break;
					case 37:
						listroleFeatures[i].SortOrder = 19;
						break;
					case 38:
						listroleFeatures[i].SortOrder = 18;
						break;
					case 39:
						listroleFeatures[i].SortOrder = 25;
						break;
					case 40:
						listroleFeatures[i].SortOrder = 14;
						break;
					case 41:
						listroleFeatures[i].SortOrder = 27;
						break;
					case 42:
						listroleFeatures[i].SortOrder = 24;
						break;
					case 43:
						listroleFeatures[i].SortOrder = 12;
						break;
					case 44:
						listroleFeatures[i].SortOrder = 20;
						break;
					case 45:
						listroleFeatures[i].SortOrder = 22;
						break;
					case 46:
						listroleFeatures[i].SortOrder = 31;
						break;
					case 47:
						listroleFeatures[i].SortOrder = 26;
						break;
					case 48:
						listroleFeatures[i].SortOrder = 17;
						break;
					case 49:
						listroleFeatures[i].SortOrder = 15;
						break;
					case 50:
						listroleFeatures[i].SortOrder = 16;
						break;
					case 51:
						listroleFeatures[i].SortOrder = 23;
						break;
					case 52:
						listroleFeatures[i].SortOrder = 21;
						break;
					case 53:
						listroleFeatures[i].SortOrder = 29;
						break;
					case 54:
						listroleFeatures[i].SortOrder = 28;
						break;
					case 55:
						listroleFeatures[i].SortOrder = 13;
						break;
					//59-60
					case 56:
						listroleFeatures[i].SortOrder = 59;
						break;
					case 57:
						listroleFeatures[i].SortOrder = 60;
						break;
					//11-10
					case 58:
						listroleFeatures[i].SortOrder = 11;
						break;
					case 59:
						listroleFeatures[i].SortOrder = 10;
						break;
				}
			}
			listroleFeatures.Sort((s1, s2) => s1.SortOrder.CompareTo(s2.SortOrder));
			return listroleFeatures;
		}

		public RoleFeaturesViewModel GetRoleFeatures(string roleId)
		{
			var result = new RoleFeaturesViewModel();
			result.IsAdd = true;
			result.Role.Id = roleId;
			// set role name
			var role = _roleRepository.Query(r => r.Id == roleId).FirstOrDefault();
			if (role != null)
			{
				result.Role.Name = role.Name;
				result.Role.RoleIndex = FindIndex(roleId);
				result.IsAdd = false;
			}

			result.AuthorizedRoleFeatureList = GetAuthorizedRoleFeatures(roleId);

			return result;
		}

		private int FindIndex(string code)
		{
			var data = _roleRepository.GetAllQueryable();
			var index = 0;
			var totalRecords = data.Count();
			var halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
			var loopCapacity = 100;
			var recordsToSkip = 0;
			if (totalRecords > 0)
			{
				var nextIteration = true;
				while (nextIteration)
				{
					for (var counter = 0; counter < 2; counter++)
					{
						recordsToSkip = recordsToSkip + (counter * halfCount);

						if (data.OrderBy("Id descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.Id == code))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("Id descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.Id == code)
								{
									index = index + 1;
									index = recordsToSkip + index;
									break;
								}
								index = index + 1;
							}
							nextIteration = false;
							break;
						}
					}
				}
			}
			return index;
		}

		public void UpdateRoleFeatures(RoleFeaturesViewModel data)
		{
			var roleId = data.Role.Id;
			var currRole = _roleRepository.Query(p => p.Id == roleId).FirstOrDefault();
			if (currRole != null)
			{
				currRole.Name = data.Role.Name;
				_roleRepository.Update(currRole);
				SaveRoleFeatures();
			}
			else
			{
				var roleInsert = new IdentityRole();
				roleInsert.Id = roleId;
				roleInsert.Name = data.Role.Name;
				_roleRepository.Add(roleInsert);
				SaveRoleFeatures();
			}
			//// delete role
			//_roleRepository.Delete(r => r.Id == roleId);
			//SaveRoleFeatures();

			//// add new role
			//var roleInsert = new IdentityRole();
			//roleInsert.Id = roleId;
			//roleInsert.Name = data.Role.Name;
			//_roleRepository.Add(roleInsert);
			//SaveRoleFeatures();

			// delete role features
			_roleFeaturesRepository.Delete(r => r.RoleId == roleId);
			SaveRoleFeatures();

			// add new role features
			var count = data.RoleFeaturesList.Count;
			for (var iloop = 0; iloop < count; iloop++)
			{
				var roleFeatures = new RoleFeatures();
				roleFeatures.RoleId = roleId;
				roleFeatures.UnusedFeatureC = data.RoleFeaturesList[iloop].UnusedFeatureC;
				_roleFeaturesRepository.Add(roleFeatures);
			}

			SaveRoleFeatures();
		}

		public void DeleteRoleFeatures(string id)
		{
			_roleFeaturesRepository.Delete(r => r.RoleId == id);
			_roleRepository.Delete(r => r.Id == id);

			SaveRoleFeatures();
		}
	}
}