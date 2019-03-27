using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.Container;
using Website.ViewModels.Customer;
using Website.Enum;

namespace Service.Services
{
	public interface IContainerTypeService
	{
		IEnumerable<ContainerTypeViewModel> GetContainerTypes();
		IEnumerable<ContainerTypeViewModel> GetContainerTypesByCode(string value);
		IEnumerable<ContainerTypeViewModel> GetContainerTypes(string value);
		ContainerTypeStatusViewModel GetContainerTypeByCode(string mainCode);

		ContainerTypeDatatables GetContainerTypesForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			 string custSearchValue);
		void CreateContainerType(ContainerTypeViewModel containertype);
		void UpdateContainerType(ContainerTypeViewModel containertype);
		void DeleteContainerType(string id);
		void SetStatusContainerType(string id);
		ContainerTypeViewModel GetByName(string value);
		void SaveContainerType();
	}

	public class ContainerTypeService : IContainerTypeService
	{
		private readonly IContainerTypeRepository _containertypeRepository;
		private readonly IUnitOfWork _unitOfWork;

		public ContainerTypeService(IContainerTypeRepository containertypeRepository, IUnitOfWork unitOfWork)
		{
			this._containertypeRepository = containertypeRepository;
			this._unitOfWork = unitOfWork;
		}

		#region IContainerTypeService members
		public IEnumerable<ContainerTypeViewModel> GetContainerTypes()
		{
            var source = _containertypeRepository.Query(i => i.IsActive == Constants.ACTIVE);
			var destination = Mapper.Map<IEnumerable<ContainerType_M>, IEnumerable<ContainerTypeViewModel>>(source);
			return destination;
		}

		public IEnumerable<ContainerTypeViewModel> GetContainerTypes(string value)
		{
			var containertype = _containertypeRepository.Query(i => (i.ContainerTypeC.Contains(value) ||
																			i.ContainerTypeN.Contains(value)) && 
                                                                            i.IsActive == Constants.ACTIVE);
			//var containertype = _containertypeRepository.Query(cus => cus.ContainerTypeC == value);
			var destination = Mapper.Map<IEnumerable<ContainerType_M>, IEnumerable<ContainerTypeViewModel>>(containertype);
			return destination;
		}

		public IEnumerable<ContainerTypeViewModel> GetContainerTypesByCode(string value)
		{
			var containertype = _containertypeRepository.Query(i => i.ContainerTypeC.StartsWith(value));
			var destination = Mapper.Map<IEnumerable<ContainerType_M>, IEnumerable<ContainerTypeViewModel>>(containertype);
			return destination;
		}

		public ContainerTypeStatusViewModel GetContainerTypeByCode(string code)
		{
			var containertypeStatus = new ContainerTypeStatusViewModel();
			var containertype = _containertypeRepository.Query(cus => cus.ContainerTypeC == code).FirstOrDefault();
			if (containertype != null)
			{
				var containerViewModel = Mapper.Map<ContainerType_M, ContainerTypeViewModel>(containertype);
				containerViewModel.ContainerTypeIndex = FindIndex(code);
				containertypeStatus.ContainerType = containerViewModel;
				containertypeStatus.Status = CustomerStatus.Edit.ToString();
			}
			else
			{
				containertypeStatus.Status = CustomerStatus.Add.ToString();
			}
			return containertypeStatus;
		}

		public ContainerTypeDatatables GetContainerTypesForTable(int page, int itemsPerPage, string sortBy, bool reverse, string custSearchValue)
		{
			var containertypes = _containertypeRepository.GetAllQueryable();
			// searching
			if (!string.IsNullOrWhiteSpace(custSearchValue))
			{
				custSearchValue = custSearchValue.ToLower();
				containertypes = containertypes.Where(cus => cus.ContainerTypeN.ToLower().Contains(custSearchValue)
																|| cus.ContainerTypeC.ToLower().Contains(custSearchValue)
																);
			}

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			//customers = customers.AsQueryable().OrderBy("\"" + sortBy + (reverse ? " desc\"" : " asc\"")).AsEnumerable();
			var customersOrdered = containertypes.OrderBy(sortBy + (reverse ? " descending" : ""));

			// paging
			var customersPaged = customersOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var destination = Mapper.Map<List<ContainerType_M>, List<ContainerTypeViewModel>>(customersPaged);
			var custDatatable = new ContainerTypeDatatables()
			{
				Data = destination,
				Total = containertypes.Count()
			};
			return custDatatable;
		}

		public void CreateContainerType(ContainerTypeViewModel customerViewModel)
		{
			var customer = Mapper.Map<ContainerTypeViewModel, ContainerType_M>(customerViewModel);
			_containertypeRepository.Add(customer);
			SaveContainerType();
		}

		public void UpdateContainerType(ContainerTypeViewModel customer)
		{
			var customerToRemove = _containertypeRepository.GetById(customer.ContainerTypeC);
			var updateCustomer = Mapper.Map<ContainerTypeViewModel, ContainerType_M>(customer);
			_containertypeRepository.Delete(customerToRemove);
			_containertypeRepository.Add(updateCustomer);
			SaveContainerType();
		}

		//using for active and deactive user
		public void SetStatusContainerType(string id)
		{
			var containerToRemove = _containertypeRepository.Get(c => c.ContainerTypeC == id);
			if (containerToRemove.IsActive == Constants.ACTIVE)
			{
				containerToRemove.IsActive = Constants.DEACTIVE;
			}
			else
			{
				containerToRemove.IsActive = Constants.ACTIVE;
			}
			_containertypeRepository.Update(containerToRemove);
			SaveContainerType();
		}
		public void DeleteContainerType(string id)
		{
			var containerToRemove = _containertypeRepository.Get(c => c.ContainerTypeC == id);
			if (containerToRemove != null)
			{
				_containertypeRepository.Delete(containerToRemove);
				SaveContainerType();
			}
		}

		public ContainerTypeViewModel GetByName(string value)
		{
			var item = _containertypeRepository.Query(e => e.ContainerTypeN == value).FirstOrDefault();
			if (item != null)
			{
				var destination = Mapper.Map<ContainerType_M, ContainerTypeViewModel>(item);
				return destination;
			}
			return null;
		}

		private int FindIndex(string code)
		{
			var data = _containertypeRepository.GetAllQueryable();
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

						if (data.OrderBy("ContainerTypeC descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.ContainerTypeC == code))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("ContainerTypeC descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.ContainerTypeC == code)
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

		public void SaveContainerType()
		{
			_unitOfWork.Commit();
		}
		#endregion
	}
}
