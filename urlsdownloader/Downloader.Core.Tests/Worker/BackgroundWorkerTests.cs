namespace Downloader.Core.Tests.Worker
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Queue;
    using Core.Store;
    using Core.Worker;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BackgroundWorkerTests
    {
        private readonly IJobQueue _jobQueue;
        private readonly IJobStore _jobStore;
        private readonly string _downloadPath;

        public BackgroundWorkerTests()
        {
            _jobQueue = new JobQueue();
            _jobStore = new JobStore();
            _downloadPath = Path.Combine(Directory.GetCurrentDirectory(), "Downloads");
        }

        [TestMethod]
        public async Task ExecuteAsync_Should_Download()
        {
            var job = new Job
            {
                URLs = new List<string>
                {
                    "https://www.birdnote.org/sites/default/files/styles/show_photo_square_285/public/annas-hummingbird-perch-mike-hamilton-285.jpg",
                    "https://d1ia71hq4oe7pn.cloudfront.net/photo/60395561-720px.jpg",
                    "https://lattenenechat.files.wordpress.com/2011/06/humingbird.jpg",
                    "https://www.gannett-cdn.com/-mm-/9d35a190f04c324dee2ed1c98d95ac691140efac/c=0-0-3184-1796/local/-/media/Nashville/2014/08/26/hummingbird.jpg",
                    "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRNLPZQcD-p8v9566Ftqs_sNMvQ7HSywEKEazUxL4qcK4w9VeuL"
                },
                JobStatus = JobStatus.Queued
            };

            job = await _jobStore.Save(job);
            await _jobQueue.Enqueue(new JobTask
            {
                Id = job.Id
            });

            var worker = new BackgroundWorker(_jobQueue, _jobStore);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            await worker.StartAsync(cts.Token);
            job = await _jobStore.Get(job.Id);

            while (!cts.IsCancellationRequested && job.JobStatus != JobStatus.Compleated &&
                   job.JobStatus != JobStatus.Failed)
            {
                job = await _jobStore.Get(job.Id);
                await Task.Delay(TimeSpan.FromSeconds(5));
            }

            Assert.IsTrue(job.JobStatus == JobStatus.Compleated);
            Assert.IsTrue(Directory.Exists(_downloadPath));

            var files = Directory.GetFiles(_downloadPath);

            Assert.IsTrue(files.Length == job.URLs.Count);
            Assert.IsTrue(files.All(file => new FileInfo(file).Length > 0));

        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_downloadPath))
            {
                Directory.Delete(_downloadPath, true);
            }
        }



    }
}
