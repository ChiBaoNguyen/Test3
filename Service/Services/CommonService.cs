using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Website.ViewModels.Order;
using System.Data.SqlClient;
using System.Data;

namespace Service.Services
{
	public interface ICommonService
	{
		string CheckWhenDelete(string language, Dictionary<string, string> dictionary, List<string> paramList);
	}
	public class CommonService : ICommonService
	{
		private ITextResourceRepository _textResourceRepository;
		public CommonService(ITextResourceRepository textResourceRepository)
		{
			_textResourceRepository = textResourceRepository;
		}
		public string CheckWhenDelete(string language, Dictionary<string, string> dictionary, List<string> paramList)
		{
			int intLanguage = 1;
			if (language == "vi")
			{
				intLanguage = 1;
			}
			else if (language == "jp")
			{
				intLanguage = 3;
			}
			else
			{
				intLanguage = 2;
			}

			foreach (var query in dictionary)
			{
				List<object> result;
				var parameters = GetParameters(paramList);
				if (parameters.Length > 0)
				{
					result = _textResourceRepository.ExecWithStoreProcedure(query.Value, parameters).ToList();
				}
				else
				{
					result = _textResourceRepository.ExecWithStoreProcedure(query.Value, "").ToList();
				}

				if (result.Count > 0)
				{
					var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
																 (con.TextKey == query.Key
																  )).ToList();
					if (languages.Count > 0)
					{
						return languages[0].TextValue;
					}
					else
					{
						return "";
					}
				}
			}

			return "OK";
		}

		public object[] GetParameters(List<string> paramList)
		{
			// create parameters
			var parameterParameters = new List<object>();
			if (paramList.Count > 0)
			{
				for (var iloop = 0; iloop < paramList.Count; iloop++)
				{
					parameterParameters.Add(new SqlParameter("@" + (iloop + 1), SqlDbType.VarChar) { Value = paramList[iloop] });
				}
			}

			return parameterParameters.ToArray();
		}
	}
}
