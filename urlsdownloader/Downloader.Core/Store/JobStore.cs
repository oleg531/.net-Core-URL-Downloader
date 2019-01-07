namespace Downloader.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Exceptions;
    using Store;

    public class JobStore : IJobStore
    {
        private readonly ConcurrentDictionary<string, Job> _jobs;

        public JobStore()
        {
            _jobs = new ConcurrentDictionary<string, Job>();
        }

        // TODO A synchronous implementation for now. Have a thoughts that real Storage is usually async
        public async Task<Job> Save(Job job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            if (string.IsNullOrWhiteSpace(job.Id))
            {
                job.Id = Guid.NewGuid().ToString();
            }

            _jobs.AddOrUpdate(job.Id, job, (key, existingJob) => job);

            return job;
        }

        public async Task<Job> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (_jobs.TryGetValue(id, out var job))
            {
                return job;
            }

            throw new NotFoundException($"Could not found {nameof(Job)} with {nameof(id)} = {id}");
        }
    }
}
