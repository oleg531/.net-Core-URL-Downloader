namespace Downloader.Core.Queue
{
    using MediatR;
    public class DequeueJobContext : IRequest<JobTask>
    {
    }
}
