using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Coffee.Azure;
using Coffee.Core;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace Coffee.Workers.SaveToBlobStorage
{
	public class SaveToBlobStorageWorkerRole : WorkerEntryPoint
	{
		private SubscriptionClient _subscriptionClient;
		private bool _isStopped;		
		private CloudBlockBlob _blob;
		private readonly CoffeeHandler _coffeeHandler = new CoffeeHandler();

		public override void Run()
		{
			while (!_isStopped)
			{
				try
				{
					// Receive the message
					var receivedMessage = _subscriptionClient.Receive();
					if (receivedMessage == null) 
						continue;

					AddToBlobStorage(Deserialize.BrokeredMessage(receivedMessage));

					receivedMessage.Complete();
				}
				catch (MessagingException e)
				{
					if (!e.IsTransient)
					{
						Trace.WriteLine(e.Message);
						throw;
					}

					Thread.Sleep(10000);
				}
				catch (OperationCanceledException e)
				{
					if (!_isStopped)
					{
						Trace.WriteLine(e.Message);
						throw;
					}
				}
			}
		}

		private void AddToBlobStorage(CoffeeDataChangedEvent coffeDataChangedEvent)
		{
			_coffeeHandler.HandleEvent(coffeDataChangedEvent);
			var serializeObject = JsonConvert.SerializeObject(new { _coffeeHandler.NumberOfCups });

			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(serializeObject)))
			{
				_blob.UploadFromStream(stream);
			}
		}

		public override bool OnStart()
		{	
			_subscriptionClient = AzureHelpers.GetSubscriptionClient("scaleevents", "SaveToBlobStorage", ReceiveMode.PeekLock);			
			_blob = AzureHelpers.GetBlob("coffee", "state");									

			_isStopped = false;
			return base.OnStart();
		}

		public override void OnStop()
		{
			// Close the connection to Service Bus Queue
			_isStopped = true;
			_subscriptionClient.Close();			
			base.OnStop();
		}
	}
}
