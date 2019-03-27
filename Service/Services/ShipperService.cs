using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.Ship;
using Website.ViewModels.Customer;
using Website.Enum;

namespace Service.Services
{
	public interface IShipperService
	{
		IEnumerable<ShipperViewModel> GetShippers(string value);
		ShipperDatatables GetShipperForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			 string shipperSearchValue);
		void CreateShipper(ShipperViewModel shipper);
		ShipperStatusViewModel GetShipperSizeByCode(string shipperCode);
		void UpdateShipper(ShipperViewModel shipper);
		void DeleteShipper(string shipperCode);
		void SetStatusShipper(string id);
		IEnumerable<ShipperViewModel> GetShipperForSuggestion(string value);
		IEnumerable<ShipperViewModel> GetShippersByCode(string value);
		ShipperViewModel GetByName(string value);
		void SaveShipper();
	}
	public class ShipperService : IShipperService
	{
		private readonly IShipperRepository _shipperRepository;
		private readonly IUnitOfWork _unitOfWork;

		public ShipperService(IShipperRepository shipperRepository, IUnitOfWork unitOfWork)
		{
			this._shipperRepository = shipperRepository;
			this._unitOfWork = unitOfWork;
		}
		public IEnumerable<ShipperViewModel> GetShippers(string value)
		{
			var shippers = _shipperRepository.Query(sh => (Equals(sh.ShipperC.Contains(value) || sh.ShipperN.Contains(value))) &&
                                                    sh.IsActive == Constants.ACTIVE);
			if (shippers != null)
			{
				var destination = Mapper.Map<IEnumerable<Shipper_M>, IEnumerable<ShipperViewModel>>(shippers);
				return destination;
			}
			return null;
		}

		public ShipperDatatables GetShipperForTable(int page, int itemsPerPage, string sortBy, bool reverse, string shipperSearchValue)
		{
			var shipper = _shipperRepository.GetAllQueryable();
			// searching
			if (!string.IsNullOrWhiteSpace(shipperSearchValue))
			{
				shipperSearchValue = shipperSearchValue.ToLower();
				shipper = shipper.Where(cus => cus.ShipperC.ToLower().Contains(shipperSearchValue)
										|| cus.ShipperN.ToLower().Contains(shipperSearchValue));
			}

			var shipperOrdered = shipper.OrderBy(sortBy + (reverse ? " descending" : ""));

			// paging
			var shipperPaged = shipperOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var destination = Mapper.Map<List<Shipper_M>, List<ShipperViewModel>>(shipperPaged);
			var shipperDatatable = new ShipperDatatables()
			{
				Data = destination,
				Total = shipper.Count()
			};
			return shipperDatatable;
		}

		public void CreateShipper(ShipperViewModel shipper)
		{
			var shipperInsert = Mapper.Map<ShipperViewModel, Shipper_M>(shipper);
			_shipperRepository.Add(shipperInsert);
			SaveShipper();
		}

		public ShipperStatusViewModel GetShipperSizeByCode(string shipperCode)
		{
			var ShipperStatus = new ShipperStatusViewModel();
			var shipper = _shipperRepository.Query(cus => cus.ShipperC == shipperCode).FirstOrDefault();
			if (shipper != null)
			{
				var shipperViewModel = Mapper.Map<Shipper_M, ShipperViewModel>(shipper);
				var shipperIndex = FindIndex(shipperCode);
				shipperViewModel.ShipperIndex = shipperIndex;
				ShipperStatus.Shipper = shipperViewModel;
				ShipperStatus.Status = CustomerStatus.Edit.ToString();
			}
			else
			{
				ShipperStatus.Status = CustomerStatus.Add.ToString();
			}
			return ShipperStatus;
		}

		public void UpdateShipper(ShipperViewModel shipper)
		{
			var shipperToRemove = _shipperRepository.GetById(shipper.ShipperC);
			var updateShipper = Mapper.Map<ShipperViewModel, Shipper_M>(shipper);
			_shipperRepository.Delete(shipperToRemove);
			_shipperRepository.Add(updateShipper);
			SaveShipper();
		}

		public void DeleteShipper(string shipperCode)
		{
			var shipperToRemove = _shipperRepository.Get(c => c.ShipperC == shipperCode);
			if (shipperToRemove != null)
			{
				_shipperRepository.Delete(shipperToRemove);
				SaveShipper();
			}
		}

		public void SetStatusShipper(string id)
		{
			var shipperToRemove = _shipperRepository.Get(c => c.ShipperC == id);
			if (shipperToRemove.IsActive == Constants.ACTIVE)
			{
				shipperToRemove.IsActive = Constants.DEACTIVE;
			}
			else
			{
				shipperToRemove.IsActive = Constants.ACTIVE;
			}
			_shipperRepository.Update(shipperToRemove);
			SaveShipper();
		}

		public IEnumerable<ShipperViewModel> GetShipperForSuggestion(string value)
		{
            var shipper = _shipperRepository.Query(i => (i.ShipperC.Contains(value) || i.ShipperN.Contains(value)) &&
                                                    i.IsActive == Constants.ACTIVE);
			var destination = Mapper.Map<IEnumerable<Shipper_M>, IEnumerable<ShipperViewModel>>(shipper);
			return destination;
		}

		public IEnumerable<ShipperViewModel> GetShippersByCode(string value)
		{
			var shipper = _shipperRepository.Query(i => i.ShipperC.StartsWith(value));
			var destination = Mapper.Map<IEnumerable<Shipper_M>, IEnumerable<ShipperViewModel>>(shipper);
			return destination;
		}

		public ShipperViewModel GetByName(string value)
		{
			var shipper = _shipperRepository.Query(s => s.ShipperN.Equals(value)).FirstOrDefault();
			if (shipper != null)
			{
				var destination = Mapper.Map<Shipper_M, ShipperViewModel>(shipper);
				return destination;
			}
			return null;
		}

		private int FindIndex(string shipperC)
		{
			var shippers = _shipperRepository.GetAllQueryable();
			var index = 0;
			var totalRecords = shippers.Count();
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

						if (shippers.OrderBy("ShipperC descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.ShipperC == shipperC))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var shipper in shippers.OrderBy("ShipperC descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (shipper.ShipperC == shipperC)
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

		public void SaveShipper()
		{
			_unitOfWork.Commit();
		}
	}
}