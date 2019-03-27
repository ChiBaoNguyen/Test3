using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Common
{
	public class ResponseStatus
	{
		public bool Successful { get; set; }
		public string Message { get; set; }
		public string NewestOrderNo { get; set; }
	}
}
