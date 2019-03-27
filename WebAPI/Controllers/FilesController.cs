using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using WebAPI.Models;
using WebAPI.Providers;
using WebAPI.Results;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using Website.ViewModels.FileUpload;

namespace WebAPI.Controllers
{
	public class FilesController : ApiController
	{
		[HttpPost] // This is from System.Web.Http, and not from System.Web.Mvc
		public async Task<HttpResponseMessage> Upload()
		{
			if (!Request.Content.IsMimeMultipartContent())
			{
				this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
			}

			var provider = GetMultipartProvider();
			var result = await Request.Content.ReadAsMultipartAsync(provider);

			// On upload, files are given a generic name like "BodyPart_26d6abe1-3ae1-416a-9429-b35f15e6e5d5"
			// so this is how you can get the original file name
			var originalFileName = GetDeserializedFileName(result.FileData.First());

			// uploadedFileInfo object will give you some additional stuff like file length,
			// creation time, directory name, a few filesystem methods etc..
			var uploadedFileInfo = new FileInfo(result.FileData.First().LocalFileName);

			// Remove this line as well as GetFormData method if you're not
			// sending any form data with your upload request
			var fileUploadObj = GetFormData<FileUpload>(result);

			// Through the request response you can return an object to the Angular controller
			// You will be able to access this in the .success callback through its data attribute
			// If you want to send something to the .error callback, use the HttpStatusCode.BadRequest instead
			//var returnData = "ReturnTest";
			var returnData = GetFileNameOnDisk(result.FileData.First().LocalFileName);
			return this.Request.CreateResponse(HttpStatusCode.OK, new { returnData });
		}

		// You could extract these two private methods to a separate utility class since
		// they do not really belong to a controller class but that is up to you
		private MultipartFormDataStreamProvider GetMultipartProvider()
		{
			// IMPORTANT: replace "(tilde)" with the real tilde character
			// (our editor doesn't allow it, so I just wrote "(tilde)" instead)
			var uploadFolder = "~/Images"; // you could put this to web.config
			var root = HttpContext.Current.Server.MapPath(uploadFolder);
			Directory.CreateDirectory(root);
			//return new MultipartFormDataStreamProvider(root);
			// call override method to get original name
			return new FilenameMultipartFormDataStreamProvider(root);
		}

		// Extracts Request FormatData as a strongly typed model
		private object GetFormData<T>(MultipartFormDataStreamProvider result)
		{
			if (result.FormData.HasKeys())
			{
				var unescapedFormData = Uri.UnescapeDataString(result.FormData
					.GetValues(0).FirstOrDefault() ?? String.Empty);
				if (!String.IsNullOrEmpty(unescapedFormData))
					return JsonConvert.DeserializeObject<T>(unescapedFormData);
			}

			return null;
		}

		private string GetDeserializedFileName(MultipartFileData fileData)
		{
			var fileName = GetFileName(fileData);
			return JsonConvert.DeserializeObject(fileName).ToString();
		}

		public string GetFileName(MultipartFileData fileData)
		{
			return fileData.Headers.ContentDisposition.FileName;
		}

		public string GetFileNameOnDisk(string path)
		{
			var arr = path.Split(new string[] {"\\"}, StringSplitOptions.None);

			return arr[arr.Length - 1];
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
			var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? headers.ContentDisposition.FileName : Guid.NewGuid().ToString();
			return name.Replace("\"", string.Empty);
		}
	}
}