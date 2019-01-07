namespace Downloader.Core
{
    using System.Collections.Generic;

    public class Job
    {
        public string Id { get; set; }

        public IList<string> URLs { get; set; }

        public JobStatus JobStatus { get; set; }

        public string StatusName => JobStatus.ToString();
    }
}
