

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.Commodity;
using CommodityViewModel = Website.ViewModels.Order.CommodityViewModel;
using Website.ViewModels.Customer;
using Website.Enum;

namespace Service.Services
{
    public interface ICommodityService
    {
        IEnumerable<CommodityViewModel> GetCommodities(string value);
		IEnumerable<CommodityViewModel> GetCommodities();
	    IEnumerable<CommodityViewModel> GetCommoditiesByCode(string value);
		IEnumerable<CommodityViewModel> GetCommodityForSuggestion(string value);
		CommodityDatatables GetCommodityForTable(int page, int itemsPerPage, string sortBy, bool reverse,
		    string custSearchValue);
		void CreateCommodity(CommodityViewModel commodity);
		CommodityStatusViewModel GetCommodityByCode(string mainCode);
		void UpdateCommodity(CommodityViewModel commodity);
		void DeleteCommodity(string id);
		void SetStatusCommodity(string id);
	    CommodityViewModel GetByName(string value);
        void SaveCommodity();
    }
    public class CommodityService : ICommodityService
    {
        private readonly ICommodityRepository _commodityRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CommodityService(ICommodityRepository commodityRepository, IUnitOfWork unitOfWork)
        {
            this._commodityRepository = commodityRepository;
            this._unitOfWork = unitOfWork;
        }

		public CommodityDatatables GetCommodityForTable(int page, int itemsPerPage, string sortBy, bool reverse, string commoditySearchValue)
		{
			var commodity = _commodityRepository.GetAllQueryable();
			// searching
			if (!string.IsNullOrWhiteSpace(commoditySearchValue))
			{
				commoditySearchValue = commoditySearchValue.ToLower();
				commodity = commodity.Where(com => com.CommodityC.ToLower().Contains(commoditySearchValue)
																|| com.CommodityN.ToLower().Contains(commoditySearchValue)
																);
			}

			var commodityOrdered = commodity.OrderBy(sortBy + (reverse ? " descending" : ""));

			// paging
			var commodityPaged = commodityOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var destination = Mapper.Map<List<Commodity_M>, List<CommodityViewModel>>(commodityPaged);
			var comDatatable = new CommodityDatatables()
			{
				Data = destination,
				Total = commodity.Count()
			};
			return comDatatable;
		}

		public void CreateCommodity(CommodityViewModel commodity)
		{
			var commodityInsert = Mapper.Map<CommodityViewModel, Commodity_M>(commodity);
			_commodityRepository.Add(commodityInsert);
			SaveCommodity();
		}

        public IEnumerable<CommodityViewModel> GetCommodities(string value)
        {
				var commodities = _commodityRepository.Query(com => com.CommodityC.Contains(value) ||
																			  com.CommodityN.Contains(value));
            if (commodities != null)
            {
                var destination = Mapper.Map<IEnumerable<Commodity_M>, IEnumerable<CommodityViewModel>>(commodities);
                return destination;
            }
            return null;
        }

		public IEnumerable<CommodityViewModel> GetCommodities()
		{
			var commodities = _commodityRepository.GetAll();
			if (commodities != null)
			{
				var destination = Mapper.Map<IEnumerable<Commodity_M>, IEnumerable<CommodityViewModel>>(commodities);
				return destination;
			}
			return null;
		}

		public CommodityStatusViewModel GetCommodityByCode(string code)
		{
			var commodityStatus = new CommodityStatusViewModel();
			var commodity = _commodityRepository.Query(cus => cus.CommodityC == code).FirstOrDefault();
			if (commodity != null)
			{
				var commodityViewModel = Mapper.Map<Commodity_M, CommodityViewModel>(commodity);
				commodityViewModel.CommodityIndex = FindIndex(code);
				commodityStatus.Commodity = commodityViewModel;
				commodityStatus.Status = CustomerStatus.Edit.ToString();
			}
			else
			{
				commodityStatus.Status = CustomerStatus.Add.ToString();
			}
			return commodityStatus;
		}

		public void UpdateCommodity(CommodityViewModel commodity)
		{
			var commodityToRemove = _commodityRepository.GetById(commodity.CommodityC);
			var updateCommodity = Mapper.Map<CommodityViewModel, Commodity_M>(commodity);
			_commodityRepository.Delete(commodityToRemove);
			_commodityRepository.Add(updateCommodity);
			SaveCommodity();
		}

		public void DeleteCommodity(string id)
		{
			var commodityToRemove = _commodityRepository.Get(c => c.CommodityC == id);
			if (commodityToRemove != null)
			{
				_commodityRepository.Delete(commodityToRemove);
				SaveCommodity();
			}
		}

		public IEnumerable<CommodityViewModel> GetCommodityForSuggestion(string value)
		{
			var commodity = _commodityRepository.Query(cus => (cus.CommodityC.Contains(value) ||	cus.CommodityN.Contains(value)) &&
                                                                cus.IsActive == Constants.ACTIVE);
			var destination = Mapper.Map<IEnumerable<Commodity_M>, IEnumerable<CommodityViewModel>>(commodity);
			return destination;
		}

		public IEnumerable<CommodityViewModel> GetCommoditiesByCode(string value)
		{
			var commodity = _commodityRepository.Query(cus => cus.CommodityC.StartsWith(value));
			var destination = Mapper.Map<IEnumerable<Commodity_M>, IEnumerable<CommodityViewModel>>(commodity);
			return destination;
		}

		public void SetStatusCommodity(string id)
		{
			var commodityToRemove = _commodityRepository.Get(c => c.CommodityC == id);
			if (commodityToRemove.IsActive == Constants.ACTIVE)
			{
				commodityToRemove.IsActive = Constants.DEACTIVE;
			}
			else
			{
				commodityToRemove.IsActive = Constants.ACTIVE;
			}
			_commodityRepository.Update(commodityToRemove);
			SaveCommodity();
		}

		public CommodityViewModel GetByName(string value)
		{
			var emp = _commodityRepository.Query(e => e.CommodityN == value).FirstOrDefault();
			if (emp != null)
			{
				var destination = Mapper.Map<Commodity_M, CommodityViewModel>(emp);
				return destination;
			}
			return null;
		}

		private int FindIndex(string code)
		{
			var data = _commodityRepository.GetAllQueryable();
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

						if (data.OrderBy("CommodityC descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.CommodityC == code))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("CommodityC descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.CommodityC == code)
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

        public void SaveCommodity()
        {
            _unitOfWork.Commit();
        }
    }
}
