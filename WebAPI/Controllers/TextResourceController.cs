using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.TextResource;

namespace WebAPI.Controllers
{
	public class TextResourceController : ApiController
	{
		public ITextResourceService _textResourceService;

		public TextResourceController() { }

		public TextResourceController(ITextResourceService textResourceService)
		{
			this._textResourceService = textResourceService;
		}

		public async Task<IEnumerable<TextResourceViewModel>> Get(string screenName, string lang)
		{
			return await Task.Run(() => _textResourceService.GetTextResource(screenName, lang));
		}
	}
}
