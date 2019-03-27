using System;

namespace Root.Models.Calendar
{
	public class CalendarPlanItemForCounting
	{
		public string objectI { get; set; }
		public string objectN { get; set; }
		public string title { get; set; }
		public string type { get; set; }
		public DateTime starts_at { get; set; }
		public DateTime ends_at { get; set; }
	}
}
