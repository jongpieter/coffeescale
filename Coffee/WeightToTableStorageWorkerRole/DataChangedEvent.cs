using System.IO;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace WeightToTableStorageWorkerRole
{
	public class DataChangedEvent
	{
		public Device Device { get; set; }
		public byte[] Data { get; set; }

		public static DataChangedEvent Deserialize(BrokeredMessage receivedMessage)
		{
			using (var reader = new StreamReader(receivedMessage.GetBody<Stream>()))
			{
				return JsonConvert.DeserializeObject<DataChangedEvent>(reader.ReadToEnd());
			}
		}
	}
}