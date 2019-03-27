using System;
using System.Collections.Generic;
using Website.ViewModels.Container;
using Website.ViewModels.Order;

namespace Website.ViewModels.Dispatch
{
	public class DispatchDataRow
	{
		public ContainerViewModel OrderD { get; set; }
		public OrderViewModel OrderH { get; set; }
		public List<DispatchViewModel> DispatchDList { get; set; }
	}
}