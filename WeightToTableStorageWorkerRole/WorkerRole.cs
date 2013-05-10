using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace WeightToTableStorageWorkerRole
{
	public class WorkerRole : RoleEntryPoint
	{
		private const string QueueName = "scaleevents";
		private const string SubscriptionName = "WeightToTableStorage";
		private SubscriptionClient subscriptionClient;
		private bool isStopped;
		private CloudTable table;

		public override void Run()
		{
			while (!isStopped)
			{
				try
				{
					// Receive the message
					BrokeredMessage receivedMessage = subscriptionClient.Receive();

					if (receivedMessage != null)
					{
						var dataChangedEvent = DataChangedEvent.Deserialize(receivedMessage);
						var entity = new DataChangedEventToScaleLogEntity(dataChangedEvent, receivedMessage.EnqueuedTimeUtc).ToEntity();
						table.Execute(TableOperation.Insert(entity));

						// Process the message
						Trace.WriteLine("Processed", receivedMessage.SequenceNumber.ToString());

						receivedMessage.Complete();
					}
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
					if (!isStopped)
					{
						Trace.WriteLine(e.Message);
						throw;
					}
				}
			}
		}

		public override bool OnStart()
		{
			// Set the maximum number of concurrent connections 
			ServicePointManager.DefaultConnectionLimit = 12;
			
			InitializeSubscriptionClient();
			InitializeTableStorageClient();

			isStopped = false;
			return base.OnStart();
		}

		private void InitializeTableStorageClient()
		{
			var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

			var tableClient = storageAccount.CreateCloudTableClient();
			table = tableClient.GetTableReference("ScaleLog");
			table.CreateIfNotExists();
		}

		private void InitializeSubscriptionClient()
		{
			var namespaceManager = NamespaceManager.CreateFromConnectionString(CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString"));

			if (!namespaceManager.SubscriptionExists(QueueName, SubscriptionName))			
				namespaceManager.CreateSubscription(QueueName, SubscriptionName);			

			// Initialize the connection to Service Bus Queue
			subscriptionClient = SubscriptionClient.CreateFromConnectionString(
				CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString"), QueueName, SubscriptionName, ReceiveMode.PeekLock);
		}

		public override void OnStop()
		{
			// Close the connection to Service Bus Queue
			isStopped = true;
			subscriptionClient.Close();
			base.OnStop();
		}
	}
}
