using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Root.Models;
using Website.ViewModels.MaintenanceDetail;
using Root.Data.Repository;
using Root.Data.Infrastructure;

namespace Service.Services
{
	public interface IMaintenancePlanDetailService
	{
		void UpdatePlan(List<MaintenanceDetailViewModel> data, string objectI, string code);
		void SaveMaintenancePlanDetail();
	}

	public class MaintenancePlanDetailService:IMaintenancePlanDetailService
	{
		private readonly IMaintenancePlanDetailRepository _maintenancePlanDetailRepository;
		private readonly IUnitOfWork _unitOfWork;

		public MaintenancePlanDetailService(IUnitOfWork unitOfWork, IMaintenancePlanDetailRepository maintenancePlanDetailRepository)
		{
			this._unitOfWork = unitOfWork;
			this._maintenancePlanDetailRepository = maintenancePlanDetailRepository;
		}
		public void UpdatePlan(List<MaintenanceDetailViewModel> data, string objectI, string code)
		{
			if (data.Any() == false)
			{
				var deleteItems = _maintenancePlanDetailRepository.Query(x => x.ObjectI == objectI &&
																			  x.Code == code);
				foreach (var item in deleteItems)
				{
					_maintenancePlanDetailRepository.Delete(item);
				}
			}
			else
			{
				foreach (var item in data)
				{
					var dataItem = _maintenancePlanDetailRepository.Query(
							x => x.ObjectI == objectI &&
							x.Code == code &&
							x.MaintenanceItemC == item.MaintenanceItemC).FirstOrDefault();

					if (dataItem != null)
					{
						dataItem.PlanMaintenanceD = item.NextMaintenanceD;
						dataItem.PlanMaintenanceKm = item.NextMaintenanceKm;

						_maintenancePlanDetailRepository.Update(dataItem);
					}
					else
					{
						if ((item.NoticeI == "1" && item.NextMaintenanceD != null) || (item.NoticeI == "2"&& item.NextMaintenanceKm != null))
						{
							MaintenancePlan_D addItem = new MaintenancePlan_D()
							{
								ObjectI = objectI,
								Code = code,
								MaintenanceItemC = item.MaintenanceItemC,
								PlanMaintenanceD = item.NextMaintenanceD,
								PlanMaintenanceKm = item.NextMaintenanceKm
							};

							_maintenancePlanDetailRepository.Add(addItem);
						}
						
					}
				}
			}
			SaveMaintenancePlanDetail();
		}
		public void SaveMaintenancePlanDetail()
		{
			_unitOfWork.Commit();
		}
	}
}
