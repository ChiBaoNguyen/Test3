using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.RealTimeProcessing
{
	public static class DispatchHandler
	{
		public static bool HandlerMessage(string receiveString, ref string responseString)
		{
			return true;
		}
	}

	public class DispatchDataMessage
	{
		public string ResponseMessage { get; set; }

		public bool DataProcessedSuccessfully { get; set; }
	}
}
