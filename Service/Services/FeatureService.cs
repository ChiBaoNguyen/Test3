using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.Customer;
using Website.ViewModels.Feature;
using Website.Enum;

namespace Service.Services
{
	public interface IFeatureService
	{
		IEnumerable<FeatureViewModel> GetFeatures(string value);
		IEnumerable<FeatureViewModel> GetFeaturesAutosuggest(string value);
		FeatureStatusViewModel GetFeatureByCode(string mainCode);
		FeatureViewModel GetByName(string featureN);
		FeatureDatatables GetFeaturesForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			 string custSearchValue);
		void CreateFeature(FeatureViewModel feature);
		void UpdateFeature(FeatureViewModel feature);
		void DeleteFeature(string id);
		void SaveFeature();
	}

	public class FeatureService : IFeatureService
	{
		private readonly IFeatureRepository _featureRepository;
		private readonly IUnitOfWork _unitOfWork;

		public FeatureService(IFeatureRepository featureRepository, IUnitOfWork unitOfWork)
		{
			this._featureRepository = featureRepository;
			this._unitOfWork = unitOfWork;
		}

		#region IFeatureService members
		public IEnumerable<FeatureViewModel> GetFeatures(string value)
		{
			var feature = _featureRepository.Query(f => (f.FeatureC.Contains(value) || f.FeatureN.Contains(value)));
			var destination = Mapper.Map<IEnumerable<Feature_M>, IEnumerable<FeatureViewModel>>(feature);
			return destination;
		}

		public IEnumerable<FeatureViewModel> GetFeaturesAutosuggest(string value)
		{
			var features = _featureRepository.Query(i => i.FeatureC.StartsWith(value));
			var destination = Mapper.Map<IEnumerable<Feature_M>, IEnumerable<FeatureViewModel>>(features);
			return destination;
		}

		public FeatureStatusViewModel GetFeatureByCode(string code)
		{
			var featureStatus = new FeatureStatusViewModel();
			var feature = _featureRepository.Query(f => f.FeatureC == code).FirstOrDefault();
			if (feature != null)
			{
				var featureViewModel = Mapper.Map<Feature_M, FeatureViewModel>(feature);
				featureViewModel.FeatureIndex = FindIndex(code);
				featureStatus.Feature = featureViewModel;
				featureStatus.Status = CustomerStatus.Edit.ToString();
			}
			else
			{
				featureStatus.Status = CustomerStatus.Add.ToString();
			}
			return featureStatus;
		}

		public FeatureDatatables GetFeaturesForTable(int page, int itemsPerPage, string sortBy, bool reverse, string custSearchValue)
		{
			var features = _featureRepository.GetAllQueryable();
			// searching
			if (!string.IsNullOrWhiteSpace(custSearchValue))
			{
				custSearchValue = custSearchValue.ToLower();
				features = features.Where(f => f.FeatureC.ToLower().Contains(custSearchValue) || f.FeatureN.ToLower().Contains(custSearchValue));
			}

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			var featuresOrdered = features.OrderBy(sortBy + (reverse ? " descending" : ""));

			// paging
			var featuresPaged = featuresOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var destination = Mapper.Map<List<Feature_M>, List<FeatureViewModel>>(featuresPaged);
			var custDatatable = new FeatureDatatables()
			{
				Data = destination,
				Total = features.Count()
			};
			return custDatatable;
		}

		public void CreateFeature(FeatureViewModel featureViewModel)
		{
			var feature = Mapper.Map<FeatureViewModel, Feature_M>(featureViewModel);
			_featureRepository.Add(feature);
			SaveFeature();
		}

		public void UpdateFeature(FeatureViewModel feature)
		{
			var featureC = feature.FeatureC;
			var featureToRemove = _featureRepository.Query(f => f.FeatureC == featureC).FirstOrDefault();
			if (featureToRemove != null)
			{
				var updateFeature = Mapper.Map<FeatureViewModel, Feature_M>(feature);
				_featureRepository.Delete(featureToRemove);
				_featureRepository.Add(updateFeature);
				SaveFeature();
			}
		}

		public void DeleteFeature(string code)
		{
			var featureToRemove = _featureRepository.Query(f => f.FeatureC == code).FirstOrDefault();
			if (featureToRemove != null)
			{
				_featureRepository.Delete(featureToRemove);
				SaveFeature();
			}
		}

		public FeatureViewModel GetByName(string featureN)
		{
			var feature = _featureRepository.Query(f => f.FeatureN == featureN).FirstOrDefault();
			if (feature != null)
			{
				var destination = Mapper.Map<Feature_M, FeatureViewModel>(feature);
				return destination;
			}
			return null;
		}

		private int FindIndex(string code)
		{
			var data = _featureRepository.GetAllQueryable();
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

						if (data.OrderBy("FeatureC descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.FeatureC == code))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("FeatureC descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.FeatureC == code)
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

		public void SaveFeature()
		{
			_unitOfWork.Commit();
		}
		#endregion
	}
}