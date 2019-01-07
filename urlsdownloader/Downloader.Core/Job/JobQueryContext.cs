namespace Downloader.Core
{
    using MediatR;
    public class JobQueryContext : IRequest<JobVewModel>
    {
        public string Id { get; set; }

        public JobQueryContext(string id)
        {
            Id = id;
        }
    }
}
