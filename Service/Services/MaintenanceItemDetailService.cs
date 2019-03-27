using System.Security.Cryptography.X509Certificates;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Root.Models;
using Website.ViewModels.MaintenanceItemDetail;
using AutoMapper;
using Website.ViewModels.Model;

namespace Service.Services
{
	public interface IMaintenanceItemDetailService
	{
		IEnumerable<MaintenanceItemDetailViewModel> GetByModelC(string objectI, string modelC);

		void DeleteItems(string objectI, string modelC);
	}
	public class MaintenanceItemDetailService:IMaintenanceItemDetailService
	{
		private readonly IMaintenanceItemDetailRepository _maintenanceItemDetailRepository;
		private readonly IMaintenanceItemRepository _maintenanceItemRepository;
		private readonly IUnitOfWork _unitOfWork;

		public MaintenanceItemDetailService() { }
		public MaintenanceItemDetailService(IMaintenanceItemDetailRepository maintenanceItemDetailRepository,IMaintenanceItemRepository maintenanceItemRepository,IUnitOfWork unitOfWork)
		{
			_maintenanceItemDetailRepository = maintenanceItemDetailRepository;
			_maintenanceItemRepository = maintenanceItemRepository;
			_unitOfWork = unitOfWork;
		}
		public IEnumerable<MaintenanceItemDetailViewModel> GetByModelC(string objectI, string modelC)
		{
			var items = from a in _maintenanceItemDetailRepository.GetAllQueryable()
				join b in _maintenanceItemRepository.GetAllQueryable() on a.MaintenanceItemC equals b.MaintenanceItemC
				where (a.ModelC == modelC && a.ObjectI == objectI)
				orderby a.DisplayLineNo
				select new MaintenanceItemDetailViewModel()
				{
					ObjectI = a.ObjectI,
					ModelC = a.ModelC,
					MaintenanceItemC = a.MaintenanceItemC,
					MaintenanceItemN = b.MaintenanceItemN,
					DisplayLineNo = a.DisplayLineNo
				};

			if (items.Any())
			{
				//var destination = Mapper.Map<IEnumerable<MaintenanceItem_D>, IEnumerable<MaintenanceItemDetailViewModel>>(items);
				//return destination;
				return items;
			}
			return null;
		}

		public void SaveMaintenanceItem()
		{
			_unitOfWork.Commit();
		}


		public void DeleteItems(string objectI, string modelC)
		{
			var deleteItems =
				_maintenanceItemDetailRepository.GetAll().Where(x => x.ObjectI.Equals(objectI) && x.ModelC == modelC);

			if (deleteItems.Any())
			{
				foreach (var item in deleteItems)
				{
					_maintenanceItemDetailRepository.Delete(item);
				}

				SaveMaintenanceItem();
			}

			
		}
	}
}
