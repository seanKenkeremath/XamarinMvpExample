using System;
using System.Threading.Tasks;

namespace XamarinMvpSample
{
	public static class TaskExtensions
	{
		/// <summary>
		/// Run a Task concurrently without getting an error
		/// </summary>
		/// <param name="task">Task.</param>
		public static void RunConcurrent(this Task task)
		{
			if (task.Status == TaskStatus.Created)
			{
				task.Start();
			}
		}
	}
}
