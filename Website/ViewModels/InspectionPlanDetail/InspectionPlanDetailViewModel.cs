using System;
namespace Website.ViewModels.InspectionPlanDetail
{
	public class InspectionPlanDetailViewModel
	{
		public string ObjectI { get; set; }
		public string Code { get; set; }
		public int InspectionC { get; set; }
		public string InspectionN { get; set; }
		public DateTime InspectionPlanD { get; set; }
		public DateTime? InspectionD { get; set; }
	}
}