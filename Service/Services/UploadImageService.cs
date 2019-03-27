using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.Utilities;
using Website.ViewModels.Mobile.Dispatch;
using System.Net.Http;
using System.Web;
using Service.Core;
using System.Web.Http;
using Website.ViewModels.Mobile;
using Website.ViewModels.Mobile.UploadImage;

namespace Service.Services
{
    public interface IUploadImageService
    {
        Task<string> UploadFile(HttpRequestMessage request);
        ListImageUploadViewModel GetListImageUpload(string key);
        ListImageUploadViewModel GetListImageUploadMultiKey(string key);
        void SaveImage();
    }

    public class UploadImageService : IUploadImageService
    {
        private readonly IUploadImageRepository _uploadImageRepository;
        private readonly IDispatchRepository _dispatchRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UploadImageService(IUnitOfWork unitOfWork, IUploadImageRepository uploadImageRepository,
            IDispatchRepository dispatchRepository)
        {
            _unitOfWork = unitOfWork;
            _uploadImageRepository = uploadImageRepository;
            _dispatchRepository = dispatchRepository;
        }


        public UploadImageService(IDispatchRepository dispatchRepository)
        {
            _dispatchRepository = dispatchRepository;
            //this.workingFileFolder = @"\Files";
        }

        public async Task<string> UploadFile(HttpRequestMessage request)
        {
            var uploadFolder = "~/Images/Mobile"; // you could put this to web.config
            var provider = GetMultipartProvider(uploadFolder);

            var task = await request.Content.ReadAsMultipartAsync(provider); //  <= Saved file in server

            string fileNameImage = GetFileNameOnDisk(task.FileData.First().LocalFileName);
            string fileNameImageCutSuffix = fileNameImage.Split('.')[0];
            string[] getIdFromFileName = fileNameImageCutSuffix.Split('~');
            string orderImageKey = getIdFromFileName[0];
            var orderD = DateTime.Parse(getIdFromFileName[1]);
            string orderNo = getIdFromFileName[2];
            var detailNo = int.Parse(getIdFromFileName[3]);
            var dispatchNo = int.Parse(getIdFromFileName[4]);
            //Find dispatch by 4 key
            var dispatchList = _dispatchRepository.GetAllQueryable()
                .Where(od => od.OrderD.Equals(orderD))
                .Where(on => on.OrderNo.Equals(orderNo))
                .Where(dn => dn.DetailNo.Equals(detailNo))
                .Where(dp => dp.DispatchNo.Equals(dispatchNo)).ToList();
            var imageKeyOfDispatch = "";
            //If founded
            if (dispatchList.Count > 0)
            {
                foreach (var dp in dispatchList)
                {
                    if (string.IsNullOrEmpty(dp.OrderImageKey))
                    {
                        string dt = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
						string keyImage = orderD.ToString("yyyy-MM-dd") + "-" + orderNo + "-" + detailNo + "-" + dispatchNo + "-" + dt;
                        dp.OrderImageKey = keyImage;
                        dp.ImageCount = dp.ImageCount + 1;
                        imageKeyOfDispatch = keyImage;
                    }
                    else
                    {
                        dp.ImageCount = dp.ImageCount + 1;
                        imageKeyOfDispatch = dp.OrderImageKey;
                    }
                }
            }
            else
            {
                return "Dispatch not found";
            }

            //Save dispatch
            foreach (var dp in dispatchList)
            {
                _dispatchRepository.Update(dp);
            }
            SaveImage(); 
            string linkImage = uploadFolder+"/"+fileNameImage;
            UploadImageMobile image = new UploadImageMobile();
            image.ImageId = Guid.NewGuid();
            image.OrderImageKey = imageKeyOfDispatch;
            image.ImageLink = linkImage;
            DateTime dtTime = new DateTime();
            dtTime.ToString("yyyy/MM/dd");
            dtTime = DateTime.Now;
            image.CreatedDate = dtTime;
            _uploadImageRepository.Add(image);
            SaveImage();
            return linkImage;
        }

        public ListImageUploadViewModel GetListImageUpload(string key)
        {
            var listImage = new ListImageUploadViewModel();
            listImage.UploadImageMobiles = _uploadImageRepository.GetAllQueryable().Where(i => i.OrderImageKey.Equals(key)).ToList();
            return listImage;
        }

        public ListImageUploadViewModel GetListImageUploadMultiKey(string key)
        {
            var listImage = new ListImageUploadViewModel();
            string[] multikey = key.Split('X');
            List<UploadImageMobile> imageMobile = new List<UploadImageMobile>();
            foreach (string item in multikey)
            {
                var result = _uploadImageRepository.GetAllQueryable().Where(i => i.OrderImageKey.Equals(item)).ToList();
                imageMobile.AddRange(result);
            }
            listImage.UploadImageMobiles = imageMobile;
            return listImage;
        }
        private MultipartFormDataStreamProvider GetMultipartProvider(string uploadFolder)
        {
            var root = HttpContext.Current.Server.MapPath(uploadFolder);
            Directory.CreateDirectory(root);
            //return new MultipartFormDataStreamProvider(root);
            // call override method to get original name
            return new FilenameMultipartFormDataStreamProvider(root);
        }

        public string GetFileNameOnDisk(string path)
        {
            var arr = path.Split(new string[] {"\\"}, StringSplitOptions.None);

            return arr[arr.Length - 1];
        }

        public void SaveImage()
        {
            _unitOfWork.Commit();
        }
    }
}
public class FilenameMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
{
    public FilenameMultipartFormDataStreamProvider(string path)
        : base(path)
    {
    }

    public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
    {
        var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName)
            ? headers.ContentDisposition.FileName
            : Guid.NewGuid().ToString();
        return name.Replace("\"", string.Empty);
    }
}