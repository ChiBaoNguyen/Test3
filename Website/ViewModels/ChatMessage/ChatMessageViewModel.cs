using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.ChatMessage
{
	public class ChatMessageViewModel: Root.Models.ChatMessage
	{
		public string UserName { get; set; }
		public string DriverN { get; set; }
	}
}
