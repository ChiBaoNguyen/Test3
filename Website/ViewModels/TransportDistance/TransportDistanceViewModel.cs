using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Root.Models;

namespace Website.ViewModels.TransportDistance
{
	public class TransportDistanceViewModel : TransportDistance_M
	{
		public string FromAreaN { get; set; }
		public string ToAreaN { get; set; }
		public int TransportDistanceIndex { get; set; }
	}
}
