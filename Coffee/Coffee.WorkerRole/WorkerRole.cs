using Coffee.Azure;
using Coffee.Workers;
using Coffee.Workers.AddTableToTableStorage;

namespace Coffee.WorkerRole
{
	public class WorkerRole : ThreadedRoleEntryPoint
	{
		public override void Run()
		{
			var workers = new WorkerEntryPoint[]
			{
				new AddTableToTableStorageWorkerRole()
			};

			Run(workers);
		}
	}
}
