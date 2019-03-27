using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Service.Services.RealTimeProcessing;

namespace WebAPI
{
	public class DispatchMessageHub: Hub
	{
		public void HandleDispatchMessage()
		{
			Clients.Others.handleDispatchMessage();
		}
	}
}