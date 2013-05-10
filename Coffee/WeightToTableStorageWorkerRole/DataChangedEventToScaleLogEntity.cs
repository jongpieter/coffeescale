using System;

namespace WeightToTableStorageWorkerRole
{
	public class DataChangedEventToScaleLogEntity
	{
		private readonly DataChangedEvent dataChangedEvent;
		private readonly DateTime timeOfEvent;
		private readonly byte[] data;

		public DataChangedEventToScaleLogEntity(DataChangedEvent dataChangedEvent, DateTime timeOfEvent)
		{
			this.dataChangedEvent = dataChangedEvent;
			this.timeOfEvent = timeOfEvent;
			data = dataChangedEvent.Data;
		}

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

		private bool IsOunces { get { return WeightUnitByte == 11; } }
		private bool IsGrams { get { return WeightUnitByte == 2; } }

		private byte StatusByte { get { return data[1]; } }
		private byte WeightUnitByte { get { return data[2]; } }
		private byte WeightLsbByte { get { return data[4]; } }
		private byte WeightMsbByte { get { return data[5]; } }

		public ScaleLogEntity ToEntity()
		{
			var entity = new ScaleLogEntity(timeOfEvent)
			{
				WeightInGrams = WeightInGrams,
				Status = Status,
				SerialNumber = dataChangedEvent.Device.SerialNumber
			};
			return entity;
		}
	}
}