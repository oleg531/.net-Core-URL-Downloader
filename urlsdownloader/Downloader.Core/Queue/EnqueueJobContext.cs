namespace Downloader.Core.Queue
{
    using MediatR;
    public class EnqueueJobContext : IRequest
    {
        public string Id { get; set; }
    }
}
