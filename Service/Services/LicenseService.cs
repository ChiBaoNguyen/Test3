using System;
using System.Linq;
using AutoMapper;
using Root.Data.Repository;
using Root.Models;
using System.Collections.Generic;
using Website.ViewModels.License;
using Root.Data.Infrastructure;

namespace Service.Services
{
	public interface ILicenseService
	{
		IEnumerable<LicenseViewModel> Get();
		IEnumerable<LicenseViewModel> GetForSuggestion(string value);

		void Update(List<LicenseViewModel> licenses);

		LicenseViewModel GetByName(string licenseN);
	}
	public class LicenseService:ILicenseService
	{
		private readonly ILicenseRepository _licenseRepository;
		private readonly IUnitOfWork _unitOfWork;

		public LicenseService(ILicenseRepository licenseRepository, IUnitOfWork unitOfWork)
		{
			this._licenseRepository = licenseRepository;
			this._unitOfWork = unitOfWork;
		}
		public IEnumerable<LicenseViewModel> Get()
		{
			var licenses = _licenseRepository.GetAll().OrderBy(x => x.DisplayLineNo);

			if (licenses != null)
			{
				var destination = Mapper.Map<IEnumerable<License_M>, IEnumerable<LicenseViewModel>>(licenses);
				return destination;
			}
			return null;
		}
		public void Update(List<LicenseViewModel> newLicenses)
		{
			var licenses = _licenseRepository.GetAll();
			var maxLicenseCOde = 0;
			if (licenses.Any())
			{
				maxLicenseCOde = Convert.ToInt32(licenses.Max(x => Convert.ToInt32(x.LicenseC)));
			}
			//xoa
			foreach (var license in licenses)
			{
				if (newLicenses.Any(item=>item.LicenseC == license.LicenseC) == false)
				{
					_licenseRepository.Delete(license);
				}
			}
			//update
			foreach (var item in newLicenses)
			{


				if (string.IsNullOrEmpty(item.LicenseC))
				{
					var addItem = new License_M()
					{
						DisplayLineNo = item.DisplayLineNo,
						LicenseC = (++maxLicenseCOde).ToString(),
						LicenseN = item.LicenseN
					};
					_licenseRepository.Add(addItem);
				}
				else
				{
					var updateLicense = licenses.Where(x => x.LicenseC == item.LicenseC).FirstOrDefault();
					if (updateLicense != null)
					{
						updateLicense.DisplayLineNo = item.DisplayLineNo;
						updateLicense.LicenseN = item.LicenseN;
						_licenseRepository.Update(updateLicense);
					}
					
				}
			}
			SaveLicense();
		}
		public void SaveLicense()
		{
			_unitOfWork.Commit();
		}


		public IEnumerable<LicenseViewModel> GetForSuggestion(string value)
		{
			var licenses = _licenseRepository.Query(x => x.LicenseN.Contains(value) || x.LicenseC.Contains(value)).OrderBy(x=>x.DisplayLineNo);
			if (licenses != null)
			{
				var destination = Mapper.Map<IEnumerable<License_M>, IEnumerable<LicenseViewModel>>(licenses);
				return destination;
			}
			return null;
		}


		public LicenseViewModel GetByName(string licenseN)
		{
			if (licenseN == null) return null;
			var license = _licenseRepository.Query(x => x.LicenseN == licenseN).FirstOrDefault();
			if (license != null)
			{
				var destination = Mapper.Map<License_M, LicenseViewModel>(license);
				return destination;
			}
			return null;
		}
	}
}
