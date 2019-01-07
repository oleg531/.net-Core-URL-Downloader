namespace Downloader.Core
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Store;

    public class JobQuery : IRequestHandler<JobQueryContext, JobVewModel>
    {
        private readonly IJobStore _jobStore;

        public JobQuery(IJobStore jobStore)
        {
            _jobStore = jobStore;
        }

        public async Task<JobVewModel> Handle(JobQueryContext jobQueryContext, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(jobQueryContext.Id))
            {
                throw new ArgumentException($"Parameter {nameof(jobQueryContext.Id)} is emty.");
            }

            var job = await _jobStore.Get(jobQueryContext.Id);

            var jobViewModel = new JobVewModel
            {
                Id = job.Id,
                Status = job.StatusName
            };
            return jobViewModel;
        }
    }
}
