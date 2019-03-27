using AutoMapper;
using Root.Data.Infrastructure;
using System;
using System.Linq;
using System.Collections.Generic;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.MaintenanceItem;

namespace Service.Services
{
	public interface IMaintenanceItemService
	{
		IEnumerable<MaintenanceItemViewModel> Get();
		IEnumerable<MaintenanceItemViewModel> GetForSuggestion(string value);
		MaintenanceItemViewModel GetByName(string name);
		void Update(List<MaintenanceItemViewModel> licenses);
	}
	public class MaintenanceItemService : IMaintenanceItemService
	{
		private readonly IMaintenanceItemRepository _maintenanceItemRepository;
		private readonly IUnitOfWork _unitOfWork;
		public MaintenanceItemService(IMaintenanceItemRepository maintenanceItemRepository, IUnitOfWork unitOfWork)
		{
			_maintenanceItemRepository = maintenanceItemRepository;
			_unitOfWork = unitOfWork;
		}
		public IEnumerable<MaintenanceItemViewModel> Get()
		{
			var maintenanceItems = _maintenanceItemRepository.GetAll().OrderBy(x => x.DisplayLineNo);

			if (maintenanceItems.Any())
			{
				var destination = Mapper.Map<IEnumerable<MaintenanceItem_M>, IEnumerable<MaintenanceItemViewModel>>(maintenanceItems);
				return destination;
			}
			return null;
		}

		public void Update(List<MaintenanceItemViewModel> newMaintenaceItems)
		{
			var maintenanceItems = _maintenanceItemRepository.GetAll();
			var maxItemCode = 0;
			if (maintenanceItems.Any())
			{
				maxItemCode = maintenanceItems.Max(x => x.MaintenanceItemC);
			}
			//xoa
			foreach (var item in maintenanceItems)
			{
				if (newMaintenaceItems.Any(x => x.MaintenanceItemC == item.MaintenanceItemC) == false)
				{
					_maintenanceItemRepository.Delete(item);
				}
			}
			//update
			foreach (var item in newMaintenaceItems)
			{


				if (item.MaintenanceItemC==-1)
				{
					var addItem = new MaintenanceItem_M()
					{
						DisplayLineNo = item.DisplayLineNo,
						MaintenanceItemC = ++maxItemCode,
						MaintenanceItemN = item.MaintenanceItemN,
						NoticeI = item.NoticeI,
						NoticeNo = item.NoticeNo,
						ReplacementInterval = item.ReplacementInterval
					};
					_maintenanceItemRepository.Add(addItem);
				}
				else
				{
					var updateMaintenance = maintenanceItems.Where(x => x.MaintenanceItemC == item.MaintenanceItemC).FirstOrDefault();
					if (updateMaintenance != null)
					{
						updateMaintenance.DisplayLineNo = item.DisplayLineNo;
						updateMaintenance.MaintenanceItemN = item.MaintenanceItemN;
						updateMaintenance.ReplacementInterval = item.ReplacementInterval;
						updateMaintenance.NoticeI = item.NoticeI;
						updateMaintenance.NoticeNo = item.NoticeNo;

						_maintenanceItemRepository.Update(updateMaintenance);
					}

				}
			}
			SaveMaintenanceItem();
		}
		public void SaveMaintenanceItem()
		{
			_unitOfWork.Commit();
		}


		public IEnumerable<MaintenanceItemViewModel> GetForSuggestion(string value)
		{
			if (value == null) return null;

			var maintenanceItems = _maintenanceItemRepository
				.GetAll()
				.Where(x=>x.MaintenanceItemN.Contains(value) || x.MaintenanceItemC.ToString().Contains(value))
				.OrderBy(x => x.DisplayLineNo);

			if (maintenanceItems.Any())
			{
				var destination = Mapper.Map<IEnumerable<MaintenanceItem_M>, IEnumerable<MaintenanceItemViewModel>>(maintenanceItems);
				return destination;
			}
			return null;
		}


		public MaintenanceItemViewModel GetByName(string name)
		{
			var maintenanceItems = _maintenanceItemRepository.Query(x => x.MaintenanceItemN == name)
				.OrderBy(x => x.DisplayLineNo).FirstOrDefault();
			if (maintenanceItems != null)
			{
				var destination = Mapper.Map<MaintenanceItem_M, MaintenanceItemViewModel>(maintenanceItems);
				return destination;
			}
			return null;
		}
	}
}
