using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace Coffee.Azure
{
	public static class AzureHelpers
	{
		private const string ServicebusConnectionString = "Microsoft.ServiceBus.ConnectionString";
		private const string StorageConnectionString = "StorageConnectionString";

		public static SubscriptionClient GetSubscriptionClient(string topicPath, string subscriptionName, ReceiveMode receiveMode)
		{
			var connectionString = CloudConfigurationManager.GetSetting(ServicebusConnectionString);

			var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

			if (!namespaceManager.SubscriptionExists(topicPath, subscriptionName))
				namespaceManager.CreateSubscription(topicPath, subscriptionName);
			
			return SubscriptionClient.CreateFromConnectionString(connectionString, topicPath, subscriptionName, receiveMode);
		}

		public static CloudTable GetTable(string tableName)
		{
			var storageAccount = GetCloudStorageAccount();
			var tableClient = storageAccount.CreateCloudTableClient();
			var table = tableClient.GetTableReference(tableName);
			table.CreateIfNotExists();
			return table;
		}

		public static CloudBlockBlob GetBlob(string containerName, string blobName)
		{
			var storageAccount = GetCloudStorageAccount();
			var blobClient = storageAccount.CreateCloudBlobClient();
			var container = blobClient.GetContainerReference("coffee");
			container.CreateIfNotExists();
			container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Container });
			return container.GetBlockBlobReference("state");
		}

		private static CloudStorageAccount GetCloudStorageAccount()
		{
			return CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting(StorageConnectionString));
		}
	}
}