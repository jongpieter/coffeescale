using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Coffee.Workers.LogChangesToTableStorage
{
	public class ScaleLogEntity : TableEntity
	{
		public ScaleLogEntity(DateTime timeOfEvent)
		{
			PartitionKey = timeOfEvent.Date.ToString("yyyyMMdd");
			RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
			TimeOfEventUtc = timeOfEvent;
		}

		public ScaleLogEntity()
		{			
		}

		public string SerialNumber { get; set; }
		public int WeightInGrams { get; set; }
		public int Status { get; set; }
		public DateTime TimeOfEventUtc { get; set; }
	}
}