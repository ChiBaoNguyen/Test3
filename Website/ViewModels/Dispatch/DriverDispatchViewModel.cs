using System;
using System.Collections.Generic;

namespace Website.ViewModels.Dispatch
{
	public class DriverDispatchViewModel
	{
		public DriverDispatchViewModel()
		{
			DriverDispatchList = new List<DispatchViewModel>();
		}

		public List<DispatchViewModel> DriverDispatchList { get; set; }
	}
}