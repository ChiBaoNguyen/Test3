using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.ViewModels.Container;

namespace Website.ViewModels.Order
{
	public class OrderContainerViewModel
	{
		public OrderViewModel Order { get; set; }
		public List<ContainerViewModel> Containers { get; set; }
	}
}
