namespace Downloader.Core.Queue
{
    using MediatR;

    public class EnqueueJobCommand : RequestHandler<EnqueueJobContext>
    {
        private readonly IJobQueue _jobQueue;

        public EnqueueJobCommand(IJobQueue jobQueue)
        {
            _jobQueue = jobQueue;
        }

        protected override void Handle(EnqueueJobContext context)
        {
            var jobTask = new JobTask
            {
                Id = context.Id
            };

            _jobQueue.Enqueue(jobTask);
        }
    }
}
