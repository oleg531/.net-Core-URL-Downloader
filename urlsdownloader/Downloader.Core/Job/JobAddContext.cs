namespace Downloader.Core
{
    using System.Collections.Generic;
    using MediatR;
    public class JobAddContext : IRequest<JobVewModel>
    {
        public IList<string> URLs { get; set; }
    }
}
