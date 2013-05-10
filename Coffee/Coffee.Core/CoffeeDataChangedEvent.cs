using System;

namespace Coffee.Core
{
	public class CoffeeDataChangedEvent
	{
		private readonly byte[] data;

		private bool IsOunces { get { return WeightUnitByte == 11; } }
		private bool IsGrams { get { return WeightUnitByte == 2; } }

		private byte StatusByte { get { return data[1]; } }
		private byte WeightUnitByte { get { return data[2]; } }
		private byte WeightLsbByte { get { return data[4]; } }
		private byte WeightMsbByte { get { return data[5]; } }

		public CoffeeDataChangedEvent(byte[] data, string serialNumber, DateTime timeOfEvent)
		{
			TimeOfEvent = timeOfEvent;
			this.data = data;
			ScaleSerialNumber = serialNumber;
		}

		public DateTime TimeOfEvent { get; private set; }
		public string ScaleSerialNumber { get; private set; }

		public int WeightInGrams
		{
			get
			{
				var weight = WeightLsbByte + (WeightMsbByte * 256);

				if (IsGrams)
					return weight;
				if (IsOunces)
					return ConvertToGrams(weight);

				return 0;
			}
		}

		public byte Status
		{
			get
			{
				return StatusByte;
			}
		}

		private static int ConvertToGrams(int ounces)
		{
			return (int)Math.Round(ounces * 28.3495 / 10);
		}
	}
}