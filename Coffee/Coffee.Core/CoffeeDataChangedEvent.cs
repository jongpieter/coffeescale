using System;

namespace Coffee.Core
{
	public class CoffeeDataChangedEvent
	{
		public string SerialNumber { get; set; }
		public DateTime Date { get; set; }
		public int Status { get; set; }
		public int Weight { get; set; }
	}
}