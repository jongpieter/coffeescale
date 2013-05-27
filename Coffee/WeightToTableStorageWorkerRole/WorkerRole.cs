using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Coffee.Core;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace WeightToTableStorageWorkerRole
{
	public class WorkerRole : RoleEntryPoint
	{
		private const string QueueName = "scaleevents";
		private const string SubscriptionName = "WeightToTableStorage";
		private SubscriptionClient subscriptionClient;
		private bool isStopped;
		private CloudTable table;
		private CloudStorageAccount storageAccount;
		private CloudBlockBlob blob;
		private readonly CoffeeHandler coffeeHandler = new CoffeeHandler();

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
						var message = DeserializeBrokeredMessage(receivedMessage);
						AddToTableStorage(message);
						AddToBlobStorage(message);

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

		private void AddToBlobStorage(CoffeeDataChangedEvent coffeDataChangedEvent)
		{
			coffeeHandler.HandleEvent(coffeDataChangedEvent);
			var serializeObject = JsonConvert.SerializeObject(new { coffeeHandler.NumberOfCups });

			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(serializeObject)))
			{
				blob.UploadFromStream(stream);
			}
		}

		private void AddToTableStorage(CoffeeDataChangedEvent dataChangedEvent)
		{
			var entity = ConvertCoffeeDataChangedEventToTableEntity(dataChangedEvent);
			table.Execute(TableOperation.InsertOrReplace(entity));
		}

		public override bool OnStart()
		{
			// Set the maximum number of concurrent connections 
			ServicePointManager.DefaultConnectionLimit = 12;
			storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
			InitializeSubscriptionClient();
			InitializeTableStorageClient();
			InitializeBlobStorageClient();

			isStopped = false;
			return base.OnStart();
		}

		private void InitializeBlobStorageClient()
		{
			var blobClient = storageAccount.CreateCloudBlobClient();
			var container = blobClient.GetContainerReference("coffee");
			container.CreateIfNotExists();
			container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Container });
			blob = container.GetBlockBlobReference("state");
		}

		private void InitializeTableStorageClient()
		{
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

		public static CoffeeDataChangedEvent DeserializeBrokeredMessage(BrokeredMessage receivedMessage)
		{
			using (var reader = new StreamReader(receivedMessage.GetBody<Stream>()))
			{
				return JsonConvert.DeserializeObject<CoffeeDataChangedEvent>(reader.ReadToEnd());
			}
		}

		public static ScaleLogEntity ConvertCoffeeDataChangedEventToTableEntity(CoffeeDataChangedEvent coffeeDataChangedEvent)
		{
			var entity = new ScaleLogEntity(coffeeDataChangedEvent.Date)
			{
				WeightInGrams = coffeeDataChangedEvent.Weight,
				Status = coffeeDataChangedEvent.Status,
				SerialNumber = coffeeDataChangedEvent.SerialNumber				
			};
			return entity;
		}
	}
}
