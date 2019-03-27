using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public class ChatMessage
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public string DriverUserId { get; set; }
		public string Message { get; set; }
		public DateTime Create_At { get; set; }
		public bool IsDriver { get; set; }
	}
}
