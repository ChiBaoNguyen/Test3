using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Website.Enum;

namespace Website.Utilities
{
	public class Utilities
	{
        public static void Switch<T>(IList<T> array, int index1, int index2)
        {
            var aux = array[index1];
            array[index1] = array[index2];
            array[index2] = aux;
        }
		public static int SubstractTwoDate(DateTime date1, DateTime date2)
		{
			try
			{
				date1 = new DateTime(date1.Year, date1.Month, date1.Day);
				date2 = new DateTime(date2.Year, date2.Month, date2.Day);
				return (int)date1.Subtract(date2).TotalDays;
			}
			catch (Exception)
			{
				return 100;
			}
		}

		public static DateTime ConvertStringToDateTime(string date)
		{
			try
			{
				return Convert.ToDateTime(date);
			}
			catch (Exception)
			{
				return DateTime.Now;
			}
		}

		public static string GetOrderTypeName(string orderType)
		{
			if (orderType == Constants.EXP)
			{
				return Constants.EXPNAME;
			}
			else if (orderType == Constants.IMP)
			{
				return Constants.IMPNAME;
			}

			return Constants.ETCNAME;
		}

		public static string GetContainerSizeName(string size)
		{
			if (size == Constants.CONTAINERSIZE1)
			{
				return Constants.CONTAINERSIZE1N;
			}
			else if (size == Constants.CONTAINERSIZE2)
			{
				return Constants.CONTAINERSIZE2N;
			}
			else if (size == Constants.CONTAINERSIZE3)
			{
				return Constants.CONTAINERSIZE3N;
			}
			else
				return "";
		}

		public static string GetFormatDateReportByLanguage(DateTime? date, int language)
		{
			var result = "";
			if (date.HasValue)
			{
				switch (language)
				{
					// Vietnamese
					case 1:
						result = date.Value.ToString("dd/MM/yyyy");
						break;
					// US
					case 2:
						result = date.Value.ToString("MM/dd/yyyy");
						break;
					// Japanese
					case 3:
						//result = date.Year + "年" + ("0" + date.Month).Substring(("0" + date.Month).Length - 2) + "月" + ("0" + date.Day).Substring(("0" + date.Day).Length - 2) + "日";
						result = date.Value.ToString("yyyy/MM/dd");
						break;
				}
			}
			return result;
		}
		public static string GetFormatShortDateReportByLanguage(DateTime? date, int language)
		{
			var result = "";
			if (date.HasValue)
			{
				switch (language)
				{
					// Vietnamese
					case 1:
						result = date.Value.ToString("dd/MM");
						break;
					// US
					case 2:
						result = date.Value.ToString("MM/dd");
						break;
					// Japanese
					case 3:
						result = date.Value.ToString("MM/dd");
						break;
				}
			}
			return result;
		}
		public static string GetFormatMonthDateReportByLanguage(DateTime? date, int language)
		{
			var result = "";
			if (date.HasValue)
			{
				switch (language)
				{
					// Vietnamese
					case 1:
						result = date.Value.ToString("MM/yyyy");
						break;
					// US
					case 2:
						result = date.Value.ToString("MM/yyyy");
						break;
					// Japanese
					case 3:
						result = date.Value.ToString("yyyy/MM");
						break;
				}
			}
			return result;
		}

		public static string GetFormatDateAndHourReportByLanguage(DateTime date, int language)
		{
			var result = "";
			try
			{
				switch (language)
				{
					// Vietnamese
					case 1:
						var resultD = date.ToString("dd/MM");
						var resultT = date.ToString("HH:mm");
						if (resultT.Equals("00:00"))
						{
							result = resultD;
						}
						else
						{
							result = resultD + " " + resultT;
						}
						break;
					// US
					case 2:
						result = date.ToString("MM/dd HH:mm");
						break;
					// Japanese
					case 3:
						result = date.ToString("MM/dd HH:mm");
						break;
				}
			}
			catch (Exception)
			{
				result = "";
			}

			return result;
		}

		public static string GetHourMinuteFromDateTime(DateTime date)
		{
			var result = "";
			try
			{
				result = date.ToString("HH:mm");
			}
			catch (Exception)
			{
				result = "";
			}

			return result;
		}

		public static byte[] LoadImage(string filePath)
		{
			try
			{
				FileStream fs = new FileStream(filePath,
						   System.IO.FileMode.Open, System.IO.FileAccess.Read);
				byte[] image = new byte[fs.Length];
				fs.Read(image, 0, Convert.ToInt32(fs.Length));
				fs.Close();
				return image;
			}
			catch (Exception ex)
			{
			}

			return null;
		}

		public static decimal CalByMethodRounding(decimal number, string roundingI)
		{
			if (roundingI == "1")
			{
				return Math.Ceiling(number);
			}
			else if (roundingI == "2")
			{
				return Math.Floor(number);
			}

			return Math.Round(number);
		}

		public static Dictionary<string, string> CreateCheckDeleteQuery(string tableName, string xmlPath)
		{
			var queryList = new Dictionary<string, string>();

			//Load xml
			XDocument xdoc = XDocument.Load(xmlPath);

			//Run query
			var children = from lv1 in xdoc.Descendants(tableName)
						   select new
						   {
							   Children = lv1.Descendants()
						   };

			//Loop through results
			foreach (var child in children)
			{
				foreach (XElement des in child.Children)
				{
					var errorMess = des.Attribute("value").Value; ;
					var sql = "select top 1 * from " + des.Name + " where " + des.Value + " ";

					queryList.Add(errorMess, sql);
				}
			}

			return queryList;
		}

		public static int ConvertLanguageToInt(string languague)
		{
			int intLanguage = 2;
			if (languague == "vi")
			{
				intLanguage = 1;
			}
			else if (languague == "jp")
			{
				intLanguage = 3;
			}

			return intLanguage;
		}
	}
}