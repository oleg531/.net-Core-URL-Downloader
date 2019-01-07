namespace Downloader.Core.Queue
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    public class JobQueue : IJobQueue
    {
        private readonly ConcurrentQueue<JobTask> _queue;

        public JobQueue()
        {
            _queue = new ConcurrentQueue<JobTask>();
        }

        // TODO A synchronous implementation for now. Have a thoughts that real Queue is usually async
        public async Task Enqueue(JobTask jobTask)
        {
            if (jobTask == null)
            {
                throw new ArgumentNullException(nameof(jobTask));
            }

            _queue.Enqueue(jobTask);
            await Task.CompletedTask;
        }

        public async Task<JobTask> Dequeue()
        {
            _queue.TryDequeue(out var jobTask);
            return jobTask;
        }
    }
}
