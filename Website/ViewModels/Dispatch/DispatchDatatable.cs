using System;
using System.Collections.Generic;

namespace Website.ViewModels.Dispatch
{
	public class DispatchDatatable
	{
		public DispatchDatatable()
		{
			DispatchList = new List<DispatchDataRow>();
			Total = 0;
		}

		public List<DispatchDataRow> DispatchList { get; set; }

		public int Total { get; set; }
	}
}