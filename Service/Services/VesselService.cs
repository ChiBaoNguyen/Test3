using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.Vessel;
using Website.ViewModels.Ship;
using Website.ViewModels.Customer;
using Website.Enum;

namespace Service.Services
{
	public interface IVesselService
	{
		IEnumerable<VesselViewModel> GetVesseles(string value);
		VesselDatatables GetVesselForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			string vesselSearchValue);
		VesselStatusViewModel GetVesselByCode(string vesselCode);
		IEnumerable<VesselViewModel> GetVesselForSuggestion(string value);
		IEnumerable<VesselViewModel> GetVesselsByShippingCompanyC(string value, string shippingCompanyC);
		IEnumerable<VesselViewModel> GetVesselesByCode(string value);
		void CreateVessel(VesselViewModel vessel);
		void UpdateVessel(VesselViewModel vessel);
		void DeleteVessel(string id);
		VesselViewModel GetByName(string value);
		void SetStatusVessel(string id);
		void SaveVessel();
	}
	public class VesselService : IVesselService
	{
		private readonly IVesselRepository _vesselRepository;
		private readonly IShippingCompanyRepository _shippingCompanyRepository;
		private readonly IUnitOfWork _unitOfWork;

		public VesselService(IVesselRepository vesselRepository, IShippingCompanyRepository shippingCompanyRepository, IUnitOfWork unitOfWork)
		{
			this._vesselRepository = vesselRepository;
			this._shippingCompanyRepository = shippingCompanyRepository;
			this._unitOfWork = unitOfWork;
		}
        public IEnumerable<VesselViewModel> GetVesseles(string value)
        {
            var vesseles = _vesselRepository.Query(i => (i.VesselC.Contains(value) ||
                                                                             i.VesselN.Contains(value)) &&
                                                                             i.IsActive == Constants.ACTIVE);
            if (vesseles != null)
            {
                var destination = Mapper.Map<IEnumerable<Vessel_M>, IEnumerable<VesselViewModel>>(vesseles);
                return destination;
            }
            return null;
        }

		public VesselDatatables GetVesselForTable(int page, int itemsPerPage, string sortBy, bool reverse, string vesselSearchValue)
		{
			var vessel = from a in _vesselRepository.GetAllQueryable()
						 join b in _shippingCompanyRepository.GetAllQueryable() on a.ShippingCompanyC
							 equals b.ShippingCompanyC
						 where ((!string.IsNullOrEmpty(vesselSearchValue) &&
								 (a.VesselC.ToLower().Contains(vesselSearchValue) ||
								  a.VesselN.ToLower().Contains(vesselSearchValue) ||
								  b.ShippingCompanyN.ToLower().Contains(vesselSearchValue))
								 ) ||
								 (string.IsNullOrEmpty(vesselSearchValue) && 1 == 1)
							  )
						 select new VesselViewModel()
						 {
							 VesselC = a.VesselC,
							 VesselN = a.VesselN,
							 ShippingCompanyC = a.ShippingCompanyC,
							 ShippingCompanyN = b.ShippingCompanyN,
							 IsActive = a.IsActive,
						 };

			var vesselOrdered = vessel.OrderBy(sortBy + (reverse ? " descending" : ""));
			// paging
			var vesselPaged = vesselOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			// set final result
			var vesselDatatable = new VesselDatatables()
			{
				Data = vesselPaged,
				Total = vessel.Count()
			};
			return vesselDatatable;
		}

		public VesselStatusViewModel GetVesselByCode(string vesselCode)
		{
			var vesselStatus = new VesselStatusViewModel();
			var vessel = _vesselRepository.Query(cus => cus.VesselC == vesselCode).FirstOrDefault();
			if (vessel != null)
			{
				var vesselViewModel = Mapper.Map<Vessel_M, VesselViewModel>(vessel);
				vesselViewModel.VesselIndex = FindIndex(vesselCode);
				vesselStatus.Vessel = vesselViewModel;
				vesselStatus.Status = CustomerStatus.Edit.ToString();

				// set shipping company name
				var shippingCompanyC = vesselViewModel.ShippingCompanyC;
				var shpCompany =
					_shippingCompanyRepository.Query(s => s.ShippingCompanyC.Equals(shippingCompanyC)).FirstOrDefault();
				if (shpCompany != null)
				{
					vesselViewModel.ShippingCompanyN = shpCompany.ShippingCompanyN;
				}
			}
			else
			{
				vesselStatus.Status = CustomerStatus.Add.ToString();
			}
			return vesselStatus;
		}

		public void CreateVessel(VesselViewModel vessel)
		{
			var vesselInsert = Mapper.Map<VesselViewModel, Vessel_M>(vessel);
			_vesselRepository.Add(vesselInsert);
			SaveVessel();
		}

		public void UpdateVessel(VesselViewModel Vessel)
		{
			var vesselToRemove = _vesselRepository.GetById(Vessel.VesselC);
			var updateVessel = Mapper.Map<VesselViewModel, Vessel_M>(Vessel);
			_vesselRepository.Delete(vesselToRemove);
			_vesselRepository.Add(updateVessel);
			SaveVessel();
		}

		public void DeleteVessel(string id)
		{
			var vesselToRemove = _vesselRepository.Get(c => c.VesselC == id);
			if (vesselToRemove != null)
			{
				_vesselRepository.Delete(vesselToRemove);
				SaveVessel();
			}
		}

		public IEnumerable<VesselViewModel> GetVesselForSuggestion(string value)
		{
		    var vessel = _vesselRepository.Query(v => (v.VesselC.Contains(value) ||
		                                               v.VesselN.Contains(value)) &&
		                                               v.IsActive == Constants.ACTIVE);
			var destination = Mapper.Map<IEnumerable<Vessel_M>, IEnumerable<VesselViewModel>>(vessel);
			return destination;
		}

		public IEnumerable<VesselViewModel> GetVesselsByShippingCompanyC(string value, string shippingCompanyC)
		{
			var vessel = _vesselRepository.Query(v => (v.VesselC.Contains(value) ||
													   v.VesselN.Contains(value)) &&
													   v.ShippingCompanyC == shippingCompanyC &&
													   v.IsActive == Constants.ACTIVE);
			var destination = Mapper.Map<IEnumerable<Vessel_M>, IEnumerable<VesselViewModel>>(vessel);
			return destination;
		}

		public IEnumerable<VesselViewModel> GetVesselesByCode(string value)
		{
			var vessel = _vesselRepository.Query(v => v.VesselC.StartsWith(value));
			var destination = Mapper.Map<IEnumerable<Vessel_M>, IEnumerable<VesselViewModel>>(vessel);
			return destination;
		}

		public VesselViewModel GetByName(string value)
		{
			var vessel = _vesselRepository.Query(s => s.VesselN.Equals(value)).FirstOrDefault();
			if (vessel != null)
			{
				var destination = Mapper.Map<Vessel_M, VesselViewModel>(vessel);
				return destination;
			}
			return null;
		}

		public void SetStatusVessel(string id)
		{
			var vesselToRemove = _vesselRepository.Get(c => c.VesselC == id);
			if (vesselToRemove.IsActive == Constants.ACTIVE)
			{
				vesselToRemove.IsActive = Constants.DEACTIVE;
			}
			else
			{
				vesselToRemove.IsActive = Constants.ACTIVE;
			}
			_vesselRepository.Update(vesselToRemove);
			SaveVessel();
		}

		private int FindIndex(string code)
		{
			var data = _vesselRepository.GetAllQueryable();
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

						if (data.OrderBy("VesselC descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.VesselC == code))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("VesselC descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.VesselC == code)
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

		public void SaveVessel()
		{
			_unitOfWork.Commit();
		}
	}
}
