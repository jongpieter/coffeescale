using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Coffee.Azure
{
	public static class TableClientExtensions
	{
		public static CloudTable GetTable(string tableName)
		{
			var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
			var tableClient = storageAccount.CreateCloudTableClient();
			var table = tableClient.GetTableReference("ScaleLog");
			table.CreateIfNotExists();
			return table;
		}
	}
}