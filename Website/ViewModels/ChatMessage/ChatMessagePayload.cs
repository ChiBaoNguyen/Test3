using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.ChatMessage
{
	public class ChatMessagePayload
	{
		public string DriverC { get; set; }
		public string Message { get; set; }
		public bool IsToAll { get; set; }
	}
}
