using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.Customer;
using Website.ViewModels.Ship;
using Website.Enum;

namespace Service.Services
{
	public interface IShippingCompanyService
	{
		IEnumerable<ShippingCompanyViewModel> GetShippingCompanies(string value);
		ShippingCompanyDatatables GetShippingCompanyForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			string shippingCompanySearchValue);
		ShippingCompanyStatusViewModel GetShippingCompanyByCode(string mainCode);
		void CreateShippingCompany(ShippingCompanyViewModel shippingCompany);
		void UpdateShippingCompany(ShippingCompanyViewModel shippingCompany);
		void DeleteShippingCompany(string id);
		IEnumerable<ShippingCompanyViewModel> GetShippingCompanyForSuggestion(string value);
		ShippingCompanyViewModel GetByName(string value);
		IEnumerable<ShippingCompanyViewModel> GetShippingCompaniesByCode(string value);
		IEnumerable<string> GetShippingCompanyCodes(string value, string shippingCompanyC);
		void SetStatusShippingCompany(string id);
		void SaveShippingCompany();
	}
	public class ShippingCompanyService : IShippingCompanyService
	{
		private readonly IShippingCompanyRepository _shippingCompanyRepository;
		private readonly IUnitOfWork _unitOfWork;

		public ShippingCompanyService(IShippingCompanyRepository shippingCompanyRepository, IUnitOfWork unitOfWork)
		{
			this._shippingCompanyRepository = shippingCompanyRepository;
			this._unitOfWork = unitOfWork;
		}
		public IEnumerable<ShippingCompanyViewModel> GetShippingCompanies(string value)
		{
			var shipComs = _shippingCompanyRepository.Query(shp => shp.ShippingCompanyC.Contains(value) ||
																		  shp.ShippingCompanyN.Contains(value));
			if (shipComs != null)
			{
				var destination = Mapper.Map<IEnumerable<ShippingCompany_M>, IEnumerable<ShippingCompanyViewModel>>(shipComs);
				return destination;
			}
			return null;
		}

		public ShippingCompanyDatatables GetShippingCompanyForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			string shippingCompanySearchValue)
		{
			var shippingCompany = _shippingCompanyRepository.GetAllQueryable();
			// searching
			if (!string.IsNullOrWhiteSpace(shippingCompanySearchValue))
			{
				shippingCompanySearchValue = shippingCompanySearchValue.ToLower();
				shippingCompany = shippingCompany.Where(com => com.ShippingCompanyC.ToLower().Contains(shippingCompanySearchValue)
														|| com.ShippingCompanyN.ToLower().Contains(shippingCompanySearchValue)
						);
			}

			var shippingCompanyOrdered = shippingCompany.OrderBy(sortBy + (reverse ? " descending" : ""));

			// paging
			var shippingCompanyPaged = shippingCompanyOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var destination = Mapper.Map<List<ShippingCompany_M>, List<ShippingCompanyViewModel>>(shippingCompanyPaged);
			var shippingCompanyDatatable = new ShippingCompanyDatatables()
			{
				Data = destination,
				Total = shippingCompany.Count()
			};
			return shippingCompanyDatatable;
		}

		public ShippingCompanyStatusViewModel GetShippingCompanyByCode(string code)
		{
			var shippingCompanyStatus = new ShippingCompanyStatusViewModel();
			var shippingCompany = _shippingCompanyRepository.Query(cus => cus.ShippingCompanyC == code).FirstOrDefault();
			if (shippingCompany != null)
			{
				var shippingCompanyViewModel = Mapper.Map<ShippingCompany_M, ShippingCompanyViewModel>(shippingCompany);
				var shippingCompanIndex = FindIndex(code);
				shippingCompanyViewModel.ShippingCompanyIndex = shippingCompanIndex;
				shippingCompanyStatus.ShippingCompany = shippingCompanyViewModel;
				shippingCompanyStatus.Status = CustomerStatus.Edit.ToString();
			}
			else
			{
				shippingCompanyStatus.Status = CustomerStatus.Add.ToString();
			}
			return shippingCompanyStatus;
		}

		public void CreateShippingCompany(ShippingCompanyViewModel shippingCompany)
		{
			var commodityInsert = Mapper.Map<ShippingCompanyViewModel, ShippingCompany_M>(shippingCompany);
			_shippingCompanyRepository.Add(commodityInsert);
			SaveShippingCompany();
		}

		public void UpdateShippingCompany(ShippingCompanyViewModel shippingCompany)
		{
			var shippingCompanyToRemove = _shippingCompanyRepository.GetById(shippingCompany.ShippingCompanyC);
			var updateShippingCompany = Mapper.Map<ShippingCompanyViewModel, ShippingCompany_M>(shippingCompany);
			_shippingCompanyRepository.Delete(shippingCompanyToRemove);
			_shippingCompanyRepository.Add(updateShippingCompany);
			SaveShippingCompany();
		}

		public void DeleteShippingCompany(string id)
		{
			var shippingCompanyToRemove = _shippingCompanyRepository.Get(c => c.ShippingCompanyC == id);
			if (shippingCompanyToRemove != null)
			{
				_shippingCompanyRepository.Delete(shippingCompanyToRemove);
				SaveShippingCompany();
			}
		}

		public IEnumerable<ShippingCompanyViewModel> GetShippingCompanyForSuggestion(string value)
		{
			var shippingCompany = _shippingCompanyRepository.Query(i => (i.ShippingCompanyC.Contains(value) ||
																			i.ShippingCompanyN.Contains(value)) &&
																		i.IsActive == Constants.ACTIVE);
			var destination = Mapper.Map<IEnumerable<ShippingCompany_M>, IEnumerable<ShippingCompanyViewModel>>(shippingCompany);
			return destination;
		}

		public IEnumerable<ShippingCompanyViewModel> GetShippingCompaniesByCode(string value)
		{
			var shippingCompany = _shippingCompanyRepository.Query(i => i.ShippingCompanyC.StartsWith(value));
			var destination = Mapper.Map<IEnumerable<ShippingCompany_M>, IEnumerable<ShippingCompanyViewModel>>(shippingCompany);
			return destination;
		}

		public IEnumerable<string> GetShippingCompanyCodes(string value, string shippingCompanyC)
		{
			value = value ?? "";
			var companyContainerCodes = new List<string>();

			var shippingCompany = _shippingCompanyRepository.Query(p => p.ShippingCompanyC.Equals(shippingCompanyC)).FirstOrDefault();
			if (shippingCompany != null)
			{
				if (!String.IsNullOrEmpty(shippingCompany.ContainerCode1))
				{
					companyContainerCodes.Add(shippingCompany.ContainerCode1);
				}
				if (!String.IsNullOrEmpty(shippingCompany.ContainerCode2))
				{
					companyContainerCodes.Add(shippingCompany.ContainerCode2);
				}
				if (!String.IsNullOrEmpty(shippingCompany.ContainerCode3))
				{
					companyContainerCodes.Add(shippingCompany.ContainerCode3);
				}
				if (!String.IsNullOrEmpty(shippingCompany.ContainerCode4))
				{
					companyContainerCodes.Add(shippingCompany.ContainerCode4);
				}
				if (!String.IsNullOrEmpty(shippingCompany.ContainerCode5))
				{
					companyContainerCodes.Add(shippingCompany.ContainerCode5);
				}
				if (!String.IsNullOrEmpty(shippingCompany.ContainerCode6))
				{
					companyContainerCodes.Add(shippingCompany.ContainerCode6);
				}
				if (!String.IsNullOrEmpty(shippingCompany.ContainerCode7))
				{
					companyContainerCodes.Add(shippingCompany.ContainerCode7);
				}
				if (!String.IsNullOrEmpty(shippingCompany.ContainerCode8))
				{
					companyContainerCodes.Add(shippingCompany.ContainerCode8);
				}
				if (!String.IsNullOrEmpty(shippingCompany.ContainerCode9))
				{
					companyContainerCodes.Add(shippingCompany.ContainerCode9);
				} 
				if (!String.IsNullOrEmpty(shippingCompany.ContainerCode10))
				{
					companyContainerCodes.Add(shippingCompany.ContainerCode10);
				}
			}

			var resultCode = companyContainerCodes.Where(p => p.ToLower().Contains(value.ToLower()));

			return resultCode;
		}

		public ShippingCompanyViewModel GetByName(string value)
		{
			var shpCompany = _shippingCompanyRepository.Query(s => s.ShippingCompanyN.Equals(value)).FirstOrDefault();
			if (shpCompany != null)
			{
				var destination = Mapper.Map<ShippingCompany_M, ShippingCompanyViewModel>(shpCompany);
				return destination;
			}
			return null;
		}

		public void SetStatusShippingCompany(string id)
		{
			var shippingCompanyToRemove = _shippingCompanyRepository.Get(c => c.ShippingCompanyC == id);
			if (shippingCompanyToRemove.IsActive == Constants.ACTIVE)
			{
				shippingCompanyToRemove.IsActive = Constants.DEACTIVE;
			}
			else
			{
				shippingCompanyToRemove.IsActive = Constants.ACTIVE;
			}
			_shippingCompanyRepository.Update(shippingCompanyToRemove);
			SaveShippingCompany();
		}

		private int FindIndex(string code)
		{
			var data = _shippingCompanyRepository.GetAllQueryable();
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

						if (data.OrderBy("ShippingCompanyC descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.ShippingCompanyC == code))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("ShippingCompanyC descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.ShippingCompanyC == code)
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

		public void SaveShippingCompany()
		{
			_unitOfWork.Commit();
		}
	}
}
