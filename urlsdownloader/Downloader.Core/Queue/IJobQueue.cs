namespace Downloader.Core.Queue
{
    using System.Threading.Tasks;

    public interface IJobQueue
    {
        Task Enqueue(JobTask jobTask);
        Task<JobTask> Dequeue();
    }
}
