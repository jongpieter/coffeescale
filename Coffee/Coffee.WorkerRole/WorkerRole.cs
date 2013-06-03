using System.Net;
using Coffee.Azure;
using Coffee.Workers.LogChangesToTableStorage;
using Coffee.Workers.SaveToBlobStorage;

namespace Coffee.WorkerRole
{
	public class WorkerRole : ThreadedRoleEntryPoint
	{
		public override void Run()
		{
			var workers = new WorkerEntryPoint[]
			{
				new LogChangesToTableStorageWorkerRole(),
				new SaveToBlobStorageWorkerRole()
			};

			Run(workers);
		}

		public override bool OnStart()
		{
			// Set the maximum number of concurrent connections 
			ServicePointManager.DefaultConnectionLimit = 12;		

			return base.OnStart();
		}
	}
}
