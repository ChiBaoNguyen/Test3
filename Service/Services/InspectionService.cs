using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.ViewModels.Inspection;
using Website.ViewModels.License;

namespace Service.Services
{
	public interface IInspectionService
	{
		IEnumerable<InspectionViewModel> Get(string objectI);

		void Update(List<InspectionViewModel> inspection);

		IEnumerable<InspectionViewModel> GetForSuggestion(string textSearch, string objectI);

		InspectionViewModel GetByName(string inspectionN, string objectI);
	}
	public class InspectionService:IInspectionService
	{
		private readonly IInspectionRepository _inspectionRepository;
		private readonly IUnitOfWork _unitOfWork;

		public InspectionService(IInspectionRepository licenseRepository, IUnitOfWork unitOfWork)
		{
			this._inspectionRepository = licenseRepository;
			this._unitOfWork = unitOfWork;
		}
		public IEnumerable<InspectionViewModel> Get(string objectI)
		{
			var inspections = _inspectionRepository.GetAll().Where(x=>x.ObjectI.Equals(objectI)).OrderBy(x => x.DisplayLineNo);

			if (inspections.Any())
			{
				var destination = Mapper.Map<IEnumerable<Inspection_M>, IEnumerable<InspectionViewModel>>(inspections);
				return destination;
			}
			return null;
		}
		public void Update(List<InspectionViewModel> newInspection)
		{

			var inspections = _inspectionRepository.GetAll().Where(x=>x.ObjectI.Equals(newInspection[0].ObjectI));
			var maxLicenseCode = 0;
			if (inspections.Any())
			{
				maxLicenseCode = Convert.ToInt32(inspections.Max(x => x.InspectionC));
			}
			//xoa
			foreach (var inspection in inspections)
			{
				if (newInspection.Any(item => item.InspectionC == inspection.InspectionC && item.ObjectI == inspection.ObjectI) == false)
				{
					_inspectionRepository.Delete(inspection);
				}
			}
			//update
			foreach (var item in newInspection)
			{
				if(item.InspectionC == -1) continue;

				if (item.InspectionC == 0)
				{
					var addItem = new Inspection_M()
					{
						DisplayLineNo = item.DisplayLineNo,
						InspectionC = ++maxLicenseCode,
						InspectionN = item.InspectionN,
						NoticeNo = item.NoticeNo,
						ObjectI = item.ObjectI
					};
					_inspectionRepository.Add(addItem);
				}
				else
				{
					var updateInspection = inspections.FirstOrDefault(x => x.InspectionC == item.InspectionC && x.ObjectI == item.ObjectI);
					if (updateInspection != null)
					{
						updateInspection.DisplayLineNo = item.DisplayLineNo;
						updateInspection.InspectionN = item.InspectionN;
						updateInspection.NoticeNo = item.NoticeNo;
						_inspectionRepository.Update(updateInspection);
					}
					
				}
			}
			SaveInspection();
		}
		public void SaveInspection()
		{
			_unitOfWork.Commit();
		}


		public IEnumerable<InspectionViewModel> GetForSuggestion(string textSearch, string objectI)
		{
			var searchModel =
				_inspectionRepository.Query(p => (p.InspectionN.Contains(textSearch) || p.InspectionC.ToString().Contains(textSearch)) && p.ObjectI == objectI);

			var destination = Mapper.Map<IEnumerable<Inspection_M>, IEnumerable<InspectionViewModel>>(searchModel);
			return destination;
		}


		public InspectionViewModel GetByName(string inspectionN, string objectI)
		{
			var data = _inspectionRepository.Query(x => x.InspectionN == inspectionN && x.ObjectI == objectI);

			var destination = Mapper.Map<IEnumerable<Inspection_M>, IEnumerable<InspectionViewModel>>(data);
			return destination.FirstOrDefault();
		}
	}
}
