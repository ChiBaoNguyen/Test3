using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.SqlServer.Server;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.Container;
using Website.ViewModels.Customer;
using System.IO;
using CrystalReport.Service.ContainerSize;
using CrystalReport.FileRpt.ContainerSize;
using System.Data;

namespace Service.Services
{
	public interface IContainerSizeService
	{
		IEnumerable<ContainerSizeViewModel> GetContainerSizes();
		IEnumerable<ContainerSizeViewModel> GetContainerSizesByCode(string value);
		//ContainerSizeStatusViewModel GetContainerSizeByCode(string mainCode);
        ContainerSizeStatusViewModel GetContainerSizeByCode(string mainCode);

        ContainerSizeDatatables GetContainerSizesForTable(int page, int itemsPerPage, string sortBy, bool reverse,
             string custSearchValue);
		void CreateContainerSize(ContainerSizeViewModel containerSize);
		void UpdateContainerSize(ContainerSizeViewModel containerSize);
		void DeleteContainerSize(string id);
		void SaveContainerSize();
        void SetStatusContainerSize(string id);
		Stream GetReportContainerSizeList(string strTypeLanguage);
		Stream GetContainSizeListForExcel(string strTypeLanguage);
	}

	public class ContainerSizeService : IContainerSizeService
	{
		private readonly IContainerSizeRepository _containerSizeRepository;
		private readonly ITextResourceRepository _textResourceRepository;
		private readonly IUnitOfWork _unitOfWork;

		public ContainerSizeService(IContainerSizeRepository containerSizeRepository, ITextResourceRepository textResourceRepository, IUnitOfWork unitOfWork)
		{
			this._containerSizeRepository = containerSizeRepository;
			this._textResourceRepository = textResourceRepository;
			this._unitOfWork = unitOfWork;
		}

		#region IContainerSizeService members
		public IEnumerable<ContainerSizeViewModel> GetContainerSizes()
		{
			var source = _containerSizeRepository.GetAll();
			var destination = Mapper.Map<IEnumerable<ContainerSize_M>, IEnumerable<ContainerSizeViewModel>>(source);
			return destination;
		}

		public IEnumerable<ContainerSizeViewModel> GetContainerSizesByCode(string value)
		{
			var containerSize = _containerSizeRepository.Query(cus => cus.ContainerSizeC.Contains(value) ||
																			cus.ContainerSizeN.Contains(value));
			var destination = Mapper.Map<IEnumerable<ContainerSize_M>, IEnumerable<ContainerSizeViewModel>>(containerSize);
			return destination;
		}

        public ContainerSizeDatatables GetContainerSizesForTable(int page, int itemsPerPage, string sortBy, bool reverse, string custSearchValue)
        {
            var containersizes = _containerSizeRepository.GetAllQueryable();
            // searching
            if (!string.IsNullOrWhiteSpace(custSearchValue))
            {
                custSearchValue = custSearchValue.ToLower();
                containersizes = containersizes.Where(cus => cus.ContainerSizeN.ToLower().Contains(custSearchValue)
                                                                || cus.ContainerSizeC.ToLower().Contains(custSearchValue)
                                                                );
            }

            var customersOrdered = containersizes.OrderBy(sortBy + (reverse ? " descending" : ""));

            // paging
            var customersPaged = customersOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            var destination = Mapper.Map<List<ContainerSize_M>, List<ContainerSizeViewModel>>(customersPaged);
            var custDatatable = new ContainerSizeDatatables()
            {
                Data = destination,
                Total = containersizes.Count()
            };
            return custDatatable;
        }

        public ContainerSizeStatusViewModel GetContainerSizeByCode(string mainCode)
        {
            var containersizeStatus = new ContainerSizeStatusViewModel();
            var containersize = _containerSizeRepository.Query(cus => cus.ContainerSizeC == mainCode).FirstOrDefault();
            if (containersize != null)
            {
                var containerViewModel = Mapper.Map<ContainerSize_M, ContainerSizeViewModel>(containersize);
                containersizeStatus.ContainerSize = containerViewModel;
                containersizeStatus.Status = CustomerStatus.Edit.ToString();
            }
            else
            {
                containersizeStatus.Status = CustomerStatus.Add.ToString();
            }
            return containersizeStatus;
        }

		public void CreateContainerSize(ContainerSizeViewModel containerSize)
		{
			var containersizeInsert = Mapper.Map<ContainerSizeViewModel, ContainerSize_M>(containerSize);
            _containerSizeRepository.Add(containersizeInsert);
			SaveContainerSize();
		}

		public void UpdateContainerSize(ContainerSizeViewModel containerSize)
		{
			var containerSizeToRemove = _containerSizeRepository.GetById(containerSize.ContainerSizeC);
			var updateContainerSize = Mapper.Map<ContainerSizeViewModel, ContainerSize_M>(containerSize);
            _containerSizeRepository.Delete(containerSizeToRemove);
            _containerSizeRepository.Add(updateContainerSize);
			SaveContainerSize();
		}

        public void SetStatusContainerSize(string id)
        {
            var containerSizeToRemove = _containerSizeRepository.Get(c => c.ContainerSizeC == id);
            containerSizeToRemove.IsActive = !containerSizeToRemove.IsActive;
            _containerSizeRepository.Update(containerSizeToRemove);
            SaveContainerSize();
        }

		//using for active and deactive user
		public void DeleteContainerSize(string id)
		{
            var containerSizeToRemove = _containerSizeRepository.Get(c => c.ContainerSizeC == id);
            if (containerSizeToRemove != null)
            {
                _containerSizeRepository.Delete(containerSizeToRemove);
                SaveContainerSize();
            }
		}

        public Stream GetReportContainerSizeList(string strTypeLanguage)
        {
            Stream stream;
            DataRow row;
            _DataSet_ContainerSizeList.DataTable1DataTable dt;
	        List<ContainerSize_M> containerSizeList;
	        int intLanguage = 1;

			var containersizes = _containerSizeRepository.GetAllQueryable();
            containersizes = containersizes.OrderBy("ContainerSizeC descending");

            containerSizeList = containersizes.ToList();

            dt = new _DataSet_ContainerSizeList.DataTable1DataTable();
            foreach(ContainerSize_M item in containerSizeList)
            {
                row = dt.NewRow();
                row["ContainerSizeC"] = item.ContainerSizeC;
                row["ContainerSizeN"] = item.ContainerSizeN;
                row["Description"] = item.Description;
				row["Status"] = item.IsActive;
				//row["Status"] = "Không khả dụng";
				//if (item.IsActive)
				//{
				//	row["Status"] = "Khả dụng";
				//}
                
                dt.Rows.Add(row);
            }

			if (strTypeLanguage == "en")
			{
				intLanguage = 2;
			}
			else if (strTypeLanguage == "jp")
			{
				intLanguage = 3;
			}
	        stream = ExportPDF.GetContainerSizeList(dt, intLanguage);
            return stream;
        }

        public Stream GetContainSizeListForExcel(string strTypeLanguage)
        {
			Dictionary<string, string> dicLanguage;
	        List<TextResource_D> languageList;
			int intLanguage;

			// get language for report
			dicLanguage = new Dictionary<string, string>();

			if (strTypeLanguage == "vi")
			{
				intLanguage = 1;
				dicLanguage.Add("LBLTITLE", "DANH SÁCH KÍCH THƯỚC CONTAINER");
			}
			else if (strTypeLanguage == "jp")
			{
				intLanguage = 3;
				dicLanguage.Add("LBLTITLE", "コンテナーのサイズのリスト");
			}
			else
			{
				intLanguage = 2;
				dicLanguage.Add("LBLTITLE", "CONTAINER SIZE LIST");
			}

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
											   (con.TextKey == "LBLCONTAINERSIZECODE" ||
												con.TextKey == "LBLCONTAINERSIZENAME" ||
												con.TextKey == "LBLDESCRIPTION" ||
												con.TextKey == "LBLSTATUS"
										));

			languageList = languages.ToList();
			foreach (TextResource_D item in languageList)
			{
				dicLanguage.Add(item.TextKey, item.TextValue);
			}

            var containersizes = _containerSizeRepository.GetAllQueryable();
			var containersizesOrdered = containersizes.OrderBy("ContainerSizeC descending");
            var containersizeList = containersizesOrdered.ToList();

            return ExportExcel.GetContainerSizeList(containersizeList, dicLanguage);
        }

		public void SaveContainerSize()
		{
			_unitOfWork.Commit();
		}
		#endregion
	}
}
