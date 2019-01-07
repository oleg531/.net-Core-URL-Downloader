namespace Downloader.Core.Store
{
    using System.Threading.Tasks;

    public interface IJobStore
    {
        Task<Job> Save(Job job);
        Task<Job> Get(string id);
    }
}
