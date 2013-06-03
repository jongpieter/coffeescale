using System.IO;
using Coffee.Core;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace Coffee.Workers
{
	public class Deserialize
	{
		public static CoffeeDataChangedEvent BrokeredMessage(BrokeredMessage receivedMessage)
		{
			using (var reader = new StreamReader(receivedMessage.GetBody<Stream>()))
			{
				return JsonConvert.DeserializeObject<CoffeeDataChangedEvent>(reader.ReadToEnd());
			}
		}
	}
}