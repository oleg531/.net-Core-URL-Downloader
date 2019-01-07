namespace Downloader.Core.Worker
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Queue;
    using Store;

    public class BackgroundWorker : BackgroundService
    {
        private const string DownloadsFolder = "./Downloads";
        private const int RunItervalInSeconds = 5;
        private const int DownloadParallelismDegree = 3;
        private readonly IJobQueue _jobQueue;
        private readonly IJobStore _jobStore;

        public BackgroundWorker(IJobQueue jobQueue, IJobStore jobStore)
        {
            _jobQueue = jobQueue;
            _jobStore = jobStore;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var jobTask = await _jobQueue.Dequeue();

                if (jobTask != null)
                {
                    var directoryInfo = !Directory.Exists(DownloadsFolder)
                        ? Directory.CreateDirectory(DownloadsFolder)
                        : new DirectoryInfo(DownloadsFolder);

                    var job = await _jobStore.Get(jobTask.Id);

                    job.JobStatus = JobStatus.InProgress;
                    job = await _jobStore.Save(job);

                    var uniqueUrLs = job.URLs.Distinct().ToList();
                    var downloadResult = await DownloadResource(DownloadParallelismDegree, uniqueUrLs, async uriString =>
                    {
                        using (var client = new WebClient())
                        {
                            if (Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
                            {
                                var fileName = CreateFileName(directoryInfo, uri, job);

                                try
                                {
                                    await client.DownloadFileTaskAsync(uri, fileName);
                                }
                                catch
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }

                        return true;
                    });

                    job.JobStatus = downloadResult ? JobStatus.Compleated : JobStatus.Failed;
                    await _jobStore.Save(job);
                }

                await Task.Delay(TimeSpan.FromSeconds(RunItervalInSeconds), stoppingToken);
            }
        }

        private string CreateFileName(DirectoryInfo directoryInfo, Uri uri, Job job)
        {
            var fileName = uri.Segments.LastOrDefault() ??
                           $"Unnamed file {Guid.NewGuid().ToString()} for job id = {job.Id}";
            var fullFileName = Path.Combine(directoryInfo.FullName, fileName);
            if (File.Exists(fullFileName))
            {
                fileName = $"Duplicate {Guid.NewGuid().ToString()} {fileName}";
                fullFileName = Path.Combine(directoryInfo.FullName, fileName);
            }

            return fullFileName;
        }

        private async Task<bool> DownloadResource(int concurrencyDegree, IEnumerable<string> urls, Func<string, Task<bool>> func)
        {
            var activeTasks = new List<Task<bool>>(concurrencyDegree);
            foreach (var downloadTask in urls.Select(func))
            {
                activeTasks.Add(downloadTask);
                if (activeTasks.Count == concurrencyDegree)
                {
                    await Task.WhenAny(activeTasks);
                    if (activeTasks.Where(x => x.IsCompleted).Any(x => !x.Result))
                    {
                        return false;
                    }
                    activeTasks.RemoveAll(x => x.IsCompleted);
                }
            }

            await Task.WhenAll(activeTasks);

            return activeTasks.All(x => x.Result);
        }
    }
}
