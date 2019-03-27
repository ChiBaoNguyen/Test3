using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.Enum;
using Website.ViewModels.TextResource;

namespace Service.Services
{
	public interface ITextResourceService
	{
		IEnumerable<TextResourceViewModel> GetTextResource(string screenName, string lang);
		void SaveCustomer();
	}

	public class TextResourceService : ITextResourceService
	{
		private readonly ITextResourceRepository _textResourceRepository;
		private readonly IScreenRepository _screenRepository;
		private readonly ILanguageRepository _languageRepository;

		private readonly IUnitOfWork _unitOfWork;

		public TextResourceService(ITextResourceRepository textResourceRepository,
											IScreenRepository screenRepository,
											ILanguageRepository languageRepository,
											IUnitOfWork unitOfWork)
		{
			this._textResourceRepository = textResourceRepository;
			this._screenRepository = screenRepository;
			this._languageRepository = languageRepository;
			this._unitOfWork = unitOfWork;
		}
		public IEnumerable<TextResourceViewModel> GetTextResource(string screenName, string lang)
		{
			var language = _languageRepository.Query(lg => lg.LanguageCode.Equals(lang)).FirstOrDefault();
			//var screen = _screenRepository.Query(scr => scr.ScreenName.Contains(screenName)).FirstOrDefault();
            var screen = _screenRepository.Query(scr => scr.ScreenName == screenName).FirstOrDefault();
			if (screen == null && language == null)
			{
				return null;
			}
			IEnumerable<TextResource_D> textResource = null;
			if (language != null && screen != null)
			{
				//screen TransportConfirm using textresource from order screen, dispatch screen
				if (screen.ScreenID == Convert.ToInt32(Screen.TransportConfirm))
				{
					var dispatchScrId = Convert.ToInt32(Screen.Dispatch);
					var orderScrId = Convert.ToInt32(Screen.Order);
					textResource = _textResourceRepository.Query(
						t => t.LanguageID == language.LanguageID && (t.ScreenID == screen.ScreenID ||
																	t.ScreenID == dispatchScrId ||
																	t.ScreenID == orderScrId));
				}
				else
				{
					textResource = _textResourceRepository.Query(
						t => t.LanguageID == language.LanguageID && t.ScreenID == screen.ScreenID);
				}
			}

			if (textResource == null) return null;
			var destination = Mapper.Map<IEnumerable<TextResource_D>, IEnumerable<TextResourceViewModel>>(textResource);
			return destination;
		}

		public void SaveCustomer()
		{
			_unitOfWork.Commit();
		}
	}
}
