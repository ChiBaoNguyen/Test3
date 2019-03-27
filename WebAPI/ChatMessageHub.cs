using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace WebAPI
{
	public class ChatMessageHub: Hub
	{
		public void HandleChatMessage()
		{
			Clients.All.handleChatMessage();
		}
	}
}