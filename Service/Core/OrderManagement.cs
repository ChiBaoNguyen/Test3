using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Core
{
	public class OrderManagement
	{
		public static string FormatOrderEntryId(string oldVal, string year, string month, int noLength)
		{
			var oldYearMonth = oldVal.Substring(0,6);
			string newNo;
			if (oldYearMonth.Equals(year + month))
			{
				var orgNo = oldVal.Replace(oldYearMonth, "");
				newNo = year + month + (Convert.ToInt64(orgNo) + 1).ToString("D" + noLength);
			}
			else
			{
				newNo = year + month + (1).ToString("D" + noLength);
			}
			
			return newNo;
		}

		public static string FormatOrderEntryId_v2(string oldVal, int noLength)
		{
			return (Convert.ToInt64(oldVal) + 1).ToString("D" + noLength);
		}
	}
}
