namespace Downloader.Core
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Queue;
    using Store;

    public class JobAddCommand : IRequestHandler<JobAddContext,JobVewModel>
    {
        private readonly IJobStore _jobStore;
        private readonly IJobQueue _jobQueue; 

        public JobAddCommand(IJobStore jobStore, IJobQueue jobQueue)
        {
            _jobStore = jobStore;
            _jobQueue = jobQueue;
        }

        public async Task<JobVewModel> Handle(JobAddContext jobContext, CancellationToken cancellationToken)
        {
            var job = new Job
            {
                URLs = jobContext.URLs,
                JobStatus = JobStatus.Created
            };
            job = await _jobStore.Save(job);

            var jobTask = new JobTask
            {
                Id = job.Id
            };
            await _jobQueue.Enqueue(jobTask);

            job.JobStatus = JobStatus.Queued;
            job = await _jobStore.Save(job);

            var jobVewModel = new JobVewModel
            {
                Id = job.Id,
                Status = job.StatusName
            };

            return jobVewModel;
        }
    }
}
