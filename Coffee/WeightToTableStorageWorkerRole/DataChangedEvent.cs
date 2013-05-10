namespace WeightToTableStorageWorkerRole
{
	public class DataChangedEvent
	{
		public Device Device { get; set; }
		public byte[] Data { get; set; }
	}
}