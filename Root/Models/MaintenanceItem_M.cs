using System;
using System.Collections.Generic;

namespace Root.Models
{
	public partial class MaintenanceItem_M
	{
		public int MaintenanceItemC { get; set; }
		public int DisplayLineNo { get; set; }
		public string MaintenanceItemN { get; set; }
		public string NoticeI { get; set; }
		public int ReplacementInterval { get; set; }
		public int NoticeNo { get; set; }
	}
}