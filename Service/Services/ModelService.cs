using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Root.Data.Repository;
using Website.ViewModels.Customer;
using Website.ViewModels.Model;
using Root.Data.Infrastructure;
using Root.Models;
using AutoMapper;
using Website.ViewModels.MaintenanceItemDetail;

namespace Service.Services
{
	public interface IModelService
	{
		IEnumerable<ModelViewModel> GetModelForSuggestion(string value, string objectI);
		ModelViewModel Get(string objectI, string modelC);
		ModelDatatables GetModelsForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			 string modelSearchValue);
		void Create(ModelViewModel model);
		void Update(ModelViewModel model);
		void SaveModel();
		void Delete(string objectI, string modelC);

		ModelViewModel GetByName(string objectI, string modelN);
	}
	public class ModelService:IModelService
	{
		private readonly IModelRepository _modelRepository;
		private readonly IMaintenanceItemDetailService _maintenanceItemDetailService;
		private readonly IMaintenanceItemDetailRepository _maintenanceItemDetailRepository;
		private readonly IUnitOfWork _unitOfWork;
		public ModelService() { }

		public ModelService(IModelRepository modelRepository, IMaintenanceItemDetailService maintenanceItemDetailService
			, IMaintenanceItemDetailRepository maintenanceItemDetailRepository
			, IUnitOfWork unitOfWork)
		{
			_modelRepository = modelRepository;
			_maintenanceItemDetailService = maintenanceItemDetailService;
			_maintenanceItemDetailRepository = maintenanceItemDetailRepository;
			_unitOfWork = unitOfWork;
		}
		public IEnumerable<ModelViewModel> GetModelForSuggestion(string value, string objectI)
		{
			var searchModel =
				_modelRepository.Query(p => (p.ModelC.Contains(value) || p.ModelN.Contains(value)) && p.ObjectI == objectI);

			var destination = Mapper.Map<IEnumerable<Model_M>, IEnumerable<ModelViewModel>>(searchModel);
			return destination;
		}

		public void Create(ModelViewModel model)
		{
			var newModel = Mapper.Map<ModelViewModel, Model_M>(model);
			_modelRepository.Add(newModel);

			UpdateMaintenanceItemDetail(model);
			
			SaveModel();
		}


		public void SaveModel()
		{
			_unitOfWork.Commit();
		}


		public void Update(ModelViewModel model)
		{
			var modelToUpdate = _modelRepository.Get(x=>x.ModelC == model.ModelC && x.ObjectI == model.ObjectI);
			modelToUpdate.ModelN = model.ModelN;
			_modelRepository.Update(modelToUpdate);

			UpdateMaintenanceItemDetail(model);

			SaveModel();
		}

		private void UpdateMaintenanceItemDetail(ModelViewModel model)
		{
			List<MaintenanceItemDetailViewModel> newMaintenaceItems = model.MaintenanceItems;
			var deleteItems =
				_maintenanceItemDetailRepository.GetAll().Where(x => x.ObjectI.Equals(model.ObjectI) && x.ModelC == model.ModelC);

			if (deleteItems.Any())
			{
				foreach (var item in deleteItems)
				{
					_maintenanceItemDetailRepository.Delete(item);
				}
			}

			//add
			foreach (var item in newMaintenaceItems)
			{
				var addItem = new MaintenanceItem_D()
				{
					DisplayLineNo = item.DisplayLineNo,
					MaintenanceItemC = item.MaintenanceItemC,
					ModelC = item.ModelC,
					ObjectI = item.ObjectI
				};
				_maintenanceItemDetailRepository.Add(addItem);
			}
		}
		public ModelViewModel Get(string objectI, string modelC)
		{
			var searchModel = _modelRepository.Query(p => p.ModelC == modelC && p.ObjectI == objectI);

			var destination = Mapper.Map<IEnumerable<Model_M>, IEnumerable<ModelViewModel>>(searchModel);

			var model = destination.FirstOrDefault();
			if (model != null)
			{
				model.ModelIndex = FindIndex(objectI, modelC);
			}
			return model;
		}

		public void Delete(string objectI, string modelC)
		{
			var deleteItem = _modelRepository.Get(m => m.ModelC == modelC && m.ObjectI == objectI);
			if (deleteItem != null)
			{
				_modelRepository.Delete(deleteItem);
				_maintenanceItemDetailService.DeleteItems(objectI, modelC);
				SaveModel();
			}
		}


		public ModelDatatables GetModelsForTable(int page, int itemsPerPage, string sortBy, bool reverse, string modelSearchValue)
		{
			var models = _modelRepository.GetAllQueryable();
			// searching
			if (!string.IsNullOrWhiteSpace(modelSearchValue))
			{
				modelSearchValue = modelSearchValue.ToLower();
				models = models.Where(cus => (cus.ModelN != null && cus.ModelN.ToLower().Contains(modelSearchValue)) ||
											(cus.ModelC != null && cus.ModelC.ToLower().Contains(modelSearchValue)));
			}

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			//customers = customers.AsQueryable().OrderBy("\"" + sortBy + (reverse ? " desc\"" : " asc\"")).AsEnumerable();
			var customersOrdered = models.OrderBy(sortBy + (reverse ? " descending" : ""));

			// paging
			var customersPaged = customersOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var destination = Mapper.Map<List<Model_M>, List<ModelViewModel>>(customersPaged);
			var modelDatatable = new ModelDatatables()
			{
				Data = destination,
				Total = models.Count()
			};
			return modelDatatable;
		}

		private int FindIndex(string objectI, string modelC)
		{
			var data = _modelRepository.GetAllQueryable();
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

						if (data.OrderBy("ModelC descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.ObjectI == objectI && c.ModelC == modelC))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("ModelC descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.ModelC == modelC && entity.ObjectI == objectI)
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

		public ModelViewModel GetByName(string objectI, string modelN)
		{
			if (modelN == null) return null;
			var searchModel = _modelRepository.Query(p => p.ModelN == modelN && p.ObjectI == objectI);

			var destination = Mapper.Map<IEnumerable<Model_M>, IEnumerable<ModelViewModel>>(searchModel);
			return destination.FirstOrDefault();
		}
	}
}
