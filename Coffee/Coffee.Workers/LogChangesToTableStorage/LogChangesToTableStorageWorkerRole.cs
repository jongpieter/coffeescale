using System;
using System.Diagnostics;
using System.Threading;
using Coffee.Azure;
using Coffee.Core;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage.Table;

namespace Coffee.Workers.LogChangesToTableStorage
{
	public class LogChangesToTableStorageWorkerRole : WorkerEntryPoint
	{
		private SubscriptionClient _subscriptionClient;
		private CloudTable _table;

		private bool _isStopped;

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

					var dataChangedEvent = Deserialize.BrokeredMessage(receivedMessage);
					AddToTableStorage(dataChangedEvent);

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


		public static ScaleLogEntity ConvertCoffeeDataChangedEventToTableEntity(CoffeeDataChangedEvent coffeeDataChangedEvent)
		{
			return new ScaleLogEntity(coffeeDataChangedEvent.Date)
				{
					WeightInGrams = coffeeDataChangedEvent.Weight,
					Status = coffeeDataChangedEvent.Status,
					SerialNumber = coffeeDataChangedEvent.SerialNumber
				};
		}

		private void AddToTableStorage(CoffeeDataChangedEvent dataChangedEvent)
		{
			var entity = ConvertCoffeeDataChangedEventToTableEntity(dataChangedEvent);
			_table.Execute(TableOperation.InsertOrReplace(entity));
		}

		public override bool OnStart()
		{
			_subscriptionClient = AzureHelpers.GetSubscriptionClient("scaleevents", "WeightToTableStorage", ReceiveMode.PeekLock);
			_table = AzureHelpers.GetTable("ScaleLog");

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
