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
using Website.Enum;
using Website.ViewModels.Trailer;

namespace Service.Services
{
	public interface ITrailerService
	{

		IEnumerable<TrailerViewModel> GetTrailerForSuggestion(string value);
		IEnumerable<SuggestedWarningTrailer> GetTrailerForSuggestionWithWarning(string value);
		IEnumerable<TrailerViewModel> GetTrailersAndDriverForSuggestion(string value);
		IEnumerable<TrailerViewModel> GetTrailersByCode(string value);
		TrailerViewModel GetByName(string value);
		TrailerViewModel GetByNameWarning(string value);
		TrailerDatatables GetTrailersForTable(int page, int itemsPerPage, string sortBy, bool reverse, string trailerSearchValue, string searchPartNo);
		TrailerViewModel GetTrailerByCode(string trailerC);
		void CreateTrailer(TrailerViewModel trailer);
		void UpdateTrailer(TrailerViewModel trailer);
		void DeleteTrailer(string id);
		void SetStatusTrailer(string id);
		void SaveTrailer();

		IEnumerable<TrailerViewModel> GetAll();
        List<SuggestedWarningTrailer> MGetTrailerList();
		string GetDriverNameByTrailerCode(string value);

	}
	public class TrailerService : ITrailerService
	{
		private readonly ITrailerRepository _trailerRepository;
		private readonly IDriverRepository _driverRepository;
		private readonly IModelRepository _modelRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMaintenancePlanDetailRepository _maintenancePlanDetailRepository;
		private readonly IMaintenanceDetailRepository _maintenanceDetailRepository;
		private readonly IInspectionPlanDetailService _inspectionPlanDetailService;

		public TrailerService(ITrailerRepository trailerRepository,
							  IDriverRepository driverRepository,
							  IModelRepository modelRepository,
							  IUnitOfWork unitOfWork,
			IMaintenancePlanDetailRepository maintenancePlanDetailRepository,
			IMaintenanceDetailRepository maintenanceDetailRepository,
			IInspectionPlanDetailService inspectionPlanDetailService)
		{
			this._trailerRepository = trailerRepository;
			this._driverRepository = driverRepository;
			this._modelRepository = modelRepository;
			this._unitOfWork = unitOfWork;
			this._maintenancePlanDetailRepository = maintenancePlanDetailRepository;
			this._maintenanceDetailRepository = maintenanceDetailRepository;
			this._inspectionPlanDetailService = inspectionPlanDetailService;
		}

		public IEnumerable<TrailerViewModel> GetTrailerForSuggestion(string value)
		{
			var trailer = _trailerRepository.Query(tra => (tra.TrailerC.Contains(value) ||
														  tra.TrailerNo.Contains(value)) &&
															tra.IsActive == Constants.ACTIVE);
			var destination = Mapper.Map<IEnumerable<Trailer_M>, IEnumerable<TrailerViewModel>>(trailer);
			return destination;
		}

		public IEnumerable<TrailerViewModel> GetTrailersAndDriverForSuggestion(string value)
		{
			var trailer = from a in _trailerRepository.GetAllQueryable()
						  join b in _driverRepository.GetAllQueryable() on new { a.DriverC }
							  equals new { b.DriverC } into t1
						  from b in t1.DefaultIfEmpty()
						  where ((a.TrailerC.Contains(value) || a.TrailerNo.Contains(value))
						  )
						  select new TrailerViewModel()
						  {
							  TrailerC = a.TrailerC,
							  TrailerNo = a.TrailerNo,
							  RegisteredD = a.RegisteredD,
							  DriverC = a.DriverC,
							  DriverN = b != null ? b.LastN + " " + b.FirstN : "",
						  };
			trailer = trailer.OrderBy("TrailerNo asc");

			return trailer.ToList();
		}

		public IEnumerable<SuggestedWarningTrailer> GetTrailerForSuggestionWithWarning(string value)
		{
			var trailer = from a in _trailerRepository.GetAllQueryable()
						  join b in _driverRepository.GetAllQueryable() on a.UsingDriverC 
							equals b.DriverC into t1
						  from b in t1.DefaultIfEmpty()
						  where ((a.TrailerC.Contains(value) || a.TrailerNo.Contains(value)) &&
								(a.IsActive == Constants.ACTIVE)) 
						  select new SuggestedWarningTrailer()
						  {
							  TrailerC = a.TrailerC,
							  TrailerNo = a.TrailerNo,
							  RegisteredD = a.RegisteredD,
							  VIN = a.VIN,
							  DriverC = a.DriverC,
							  DriverN = b != null ? b.LastN + " " + b.FirstN : "",
							  IsActive = a.IsActive,
							  IsUsing = a.IsUsing,
							  UsingDriverC = a.UsingDriverC
						  };
			trailer = trailer.OrderBy("TrailerC asc");
			return trailer.ToList();
		}

		public IEnumerable<TrailerViewModel> GetTrailersByCode(string value)
		{
			var trailer = _trailerRepository.Query(tra => tra.TrailerC.StartsWith(value));
			var destination = Mapper.Map<IEnumerable<Trailer_M>, IEnumerable<TrailerViewModel>>(trailer);
			return destination;
		}

		public TrailerViewModel GetByName(string value)
		{
			var trailer = _trailerRepository.Query(t => t.TrailerNo.Equals(value)).FirstOrDefault();
			if (trailer != null)
			{
				var destination = Mapper.Map<Trailer_M, TrailerViewModel>(trailer);
				return destination;
			}
			return null;
		}

		public TrailerViewModel GetByNameWarning(string value)
		{
			var trailer = _trailerRepository.Query(t => t.TrailerNo.Equals(value)).FirstOrDefault();
			if (trailer != null)
			{
				var destination = Mapper.Map<Trailer_M, TrailerViewModel>(trailer);
				return destination;
			}
			return null;
		}

		public TrailerDatatables GetTrailersForTable(int page, int itemsPerPage, string sortBy, bool reverse, string trailerSearchValue, string searchPartNo)
		{
			var trailer = from a in _trailerRepository.GetAllQueryable()
						  join b in _driverRepository.GetAllQueryable() on new { a.DriverC }
							equals new { b.DriverC } into t1
						  from b in t1.DefaultIfEmpty()
						  where (trailerSearchValue == null ||
								 a.TrailerC.Contains(trailerSearchValue) ||
								 a.TrailerNo.Contains(trailerSearchValue)
								)
						  select new TrailerViewModel()
						  {
							  TrailerC = a.TrailerC,
							  TrailerNo = a.TrailerNo,
							  RegisteredD = a.RegisteredD,
							  VIN = a.VIN,
							  DriverC = a.DriverC,
							  DriverN = b != null ? b.LastN + " " + b.FirstN : "",
							  GrossWeight = a.GrossWeight,
							  IsActive = a.IsActive,
							  Situation = a.Situation,
							  FromDate = a.FromDate,
							  ToDate = a.ToDate
						  };
			if (searchPartNo != null)
			{
				trailer = from a in _trailerRepository.GetAllQueryable()
						  join b in _driverRepository.GetAllQueryable() on new { a.DriverC }
							equals new { b.DriverC } into t1
						  from b in t1.DefaultIfEmpty()
						  join m in _maintenanceDetailRepository.GetAllQueryable() on a.TrailerC
							  equals m.Code into t4
						  from m in t4.DefaultIfEmpty()
						  where (trailerSearchValue == null ||
								 a.TrailerC.Contains(trailerSearchValue) ||
								 a.TrailerNo.Contains(trailerSearchValue)
								) &&
								(m.ObjectI == Constants.TRAILER &&
								 m.PartNo.Contains(searchPartNo)
								)
						  select new TrailerViewModel()
						  {
							  TrailerC = a.TrailerC,
							  TrailerNo = a.TrailerNo,
							  RegisteredD = a.RegisteredD,
							  VIN = a.VIN,
							  DriverC = a.DriverC,
							  DriverN = b != null ? b.LastN + " " + b.FirstN : "",
							  GrossWeight = a.GrossWeight,
							  IsActive = a.IsActive,
							  Situation = a.Situation,
							  FromDate = a.FromDate,
							  ToDate = a.ToDate
						  };
			}
			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			var trailerOrdered = trailer.OrderBy(sortBy + (reverse ? " descending" : ""));
			// paging
			var trailerPaged = trailerOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var datatable = new TrailerDatatables()
			{
				Data = trailerPaged,
				Total = trailerOrdered.Count()
			};

			return datatable;
		}

		public void CreateTrailer(TrailerViewModel trailerViewModel)
		{
			var trailer = Mapper.Map<TrailerViewModel, Trailer_M>(trailerViewModel);
			_trailerRepository.Add(trailer);
			SaveTrailer();
		}

		public void UpdateTrailer(TrailerViewModel trailer)
		{
			var trailerToRemove = _trailerRepository.GetById(trailer.TrailerC);
			var updateTrailer = Mapper.Map<TrailerViewModel, Trailer_M>(trailer);

			_trailerRepository.Delete(trailerToRemove);
			_trailerRepository.Add(updateTrailer);

			SaveTrailer();
		}

		//using for active and deactive user
		public void SetStatusTrailer(string id)
		{
			var trailerToRemove = _trailerRepository.Get(c => c.TrailerC == id);
			if (trailerToRemove.IsActive == Constants.ACTIVE)
			{
				trailerToRemove.IsActive = Constants.DEACTIVE;
			}
			else
			{
				trailerToRemove.IsActive = Constants.ACTIVE;
			}

			_trailerRepository.Update(trailerToRemove);

			SaveTrailer();
		}

		public void DeleteTrailer(string id)
		{
			var trailerToRemove = _trailerRepository.Get(c => c.TrailerC == id);
			if (trailerToRemove != null)
			{
				_trailerRepository.Delete(trailerToRemove);

				// delete in MaintenancePlan_D
				var maintenanceItemsPlan = _maintenancePlanDetailRepository.Query(x => x.ObjectI == "1" && x.Code == id);
				if (maintenanceItemsPlan != null)
				{
					foreach (var deleteItem in maintenanceItemsPlan)
					{
						_maintenancePlanDetailRepository.Delete((deleteItem));
					}
				}
				// delete Maintenance_D
				//var maintenanceItems = _maintenanceDetailRepository.Query(x => x.ObjectI == "1" && x.Code == id);
				//if (maintenanceItems != null)
				//{
				//	foreach (var deleteItem in maintenanceItems)
				//	{
				//		_maintenanceDetailRepository.Delete(deleteItem);
				//	}
				//}

				// delete in inspectionPlan_D
				_inspectionPlanDetailService.Delete("1", id);



				SaveTrailer();
			}
		}

		public TrailerViewModel GetTrailerByCode(string trailerC)
		{
			TrailerViewModel result = new TrailerViewModel();

			var trailer = from a in _trailerRepository.GetAllQueryable()
						  join b in _driverRepository.GetAllQueryable() on new { a.DriverC }
							  equals new { b.DriverC } into t1
						  from b in t1.DefaultIfEmpty()
						  join c in _modelRepository.GetAllQueryable() on new { a.ModelC, ObjectI = "1" } equals new { c.ModelC, c.ObjectI } into t2
						  from c in t2.DefaultIfEmpty()
						  where (a.TrailerC == trailerC)
						  select new TrailerViewModel()
						  {
							  TrailerC = a.TrailerC,
							  TrailerNo = a.TrailerNo,
							  RegisteredD = a.RegisteredD,
							  VIN = a.VIN,
							  DriverC = a.DriverC,
							  DriverN = b != null ? b.LastN + " " + b.FirstN : "",
							  RetiredD = b != null ? b.RetiredD : null,
							  IsActive = a.IsActive,
							  GrossWeight = a.GrossWeight,
							  ModelC = a.ModelC,
							  ModelN = c.ModelN,
							  Situation = a.Situation,
							  FromDate = a.FromDate,
							  ToDate = a.ToDate
						  };

			if (trailer.Any())
			{
				var trailerList = trailer.ToList();
				result = trailerList[0];
				result.TrailerIndex = FindIndex(trailerC);
			}

			return result;
		}

		private int FindIndex(string code)
		{
			var data = _trailerRepository.GetAllQueryable();
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

						if (data.OrderBy("TrailerC descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.TrailerC == code))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("TrailerC descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.TrailerC == code)
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

		public void SaveTrailer()
		{
			_unitOfWork.Commit();
		}


		public IEnumerable<TrailerViewModel> GetAll()
		{
			var trailer = _trailerRepository.GetAll();
			var destination = Mapper.Map<IEnumerable<Trailer_M>, IEnumerable<TrailerViewModel>>(trailer);
			return destination;
		}


        public List<SuggestedWarningTrailer> MGetTrailerList()
        {
            var trailer = from a in _trailerRepository.GetAllQueryable()
                          join b in _driverRepository.GetAllQueryable() on a.UsingDriverC
                            equals b.DriverC into t1
                          from b in t1.DefaultIfEmpty()
                          where (a.IsActive == Constants.ACTIVE)
                          select new SuggestedWarningTrailer()
                          {
                              TrailerC = a.TrailerC,
                              TrailerNo = a.TrailerNo,
                              RegisteredD = a.RegisteredD,
                              VIN = a.VIN,
                              DriverC = a.DriverC,
                              DriverN = b != null ? b.LastN + " " + b.FirstN : "",
                              IsActive = a.IsActive,
                              IsUsing = a.IsUsing,
                              UsingDriverC = a.UsingDriverC
                          };
            trailer = trailer.OrderBy("TrailerNo asc");
            return trailer.ToList();
        }
		public string GetDriverNameByTrailerCode(string value)
		{
			string finalName = "";
			var trailer = _trailerRepository.Query(t => t.TrailerC == value).FirstOrDefault();
			if (trailer != null)
			{
				if (!string.IsNullOrEmpty(trailer.DriverC))
				{
					var driver = _driverRepository.Query(dri => dri.DriverC == trailer.DriverC).FirstOrDefault();
					if (driver != null)
					{
						finalName = driver.DriverC + " - " + driver.LastN + " " + driver.FirstN;
					}
				}
			}
			return finalName;
		}
	}
}