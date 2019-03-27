using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Service.Services;
using Website.ViewModels.Mobile.Dispatch;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Web.Http;

namespace WebAPI.Controllers
{
    public class MobileUploadImageController : MobileBaseController
    {
        public IUploadImageService _uploadImageService;
		public MobileUploadImageController()
		{
		}
        public MobileUploadImageController( IUploadImageService uploadImageService)
		{
		    this._uploadImageService = uploadImageService;
		}

        [HttpGet]
        [Route("api/MobileUploadImage/GetListImageByKey")]
        public HttpResponseMessage Get(string key)
        {
            return base.BuildSuccessResult(HttpStatusCode.OK, _uploadImageService.GetListImageUploadMultiKey(key));
        }

        [System.Web.Mvc.HttpPost]
        public async Task<string> UploadFile()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return "Unsupported media type";
            }
            var mang = Request.Content.ReadAsByteArrayAsync().Result;
            try
            {
                var photos = await _uploadImageService.UploadFile(Request);
				return photos;
            }
            catch (Exception ex)
            {
                return "Error "+ex.Message;
            }
        }

	}
}