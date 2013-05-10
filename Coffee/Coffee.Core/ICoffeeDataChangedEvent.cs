using System;

namespace Coffee.Core
{
	public interface ICoffeeDataChangedEvent
	{
		DateTime TimeOfEvent { get; }
		string ScaleSerialNumber { get; }
		int WeightInGrams { get; }
		byte Status { get; }
	}
}