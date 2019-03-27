using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace WebAPI
{
	public class NotifyMessageHub: Hub
	{
		public void HandleNotifyMessage()
		{
			Clients.All.handleNotifyMessage();
		}
	}
}