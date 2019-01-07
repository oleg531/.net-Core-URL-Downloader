namespace Downloader.Core.Queue
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    public class DequeueJobCommand : IRequestHandler<DequeueJobContext, JobTask>
    {
        private readonly IJobQueue _jobQueue;

        public DequeueJobCommand(IJobQueue jobQueue)
        {
            _jobQueue = jobQueue;
        }

        public async Task<JobTask> Handle(DequeueJobContext request, CancellationToken cancellationToken)
        {
            return await _jobQueue.Dequeue();
        }
    }
}
