namespace Downloader.Core.Tests.Store
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Store;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JobStoreTests
    {
        private readonly IJobStore _jobStore;

        public JobStoreTests()
        {
            _jobStore = new JobStore();
        }

        [TestMethod]
        public async Task Save_When_Job_Present_Should_Save()
        {
            var job = new Job();

            var savedJod = await _jobStore.Save(job);
            var expetedJob = await _jobStore.Get(savedJod.Id);

            Assert.AreEqual(savedJod, expetedJob);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Save_When_Null_Should_Throw_Argument_null_Exception()
        {
            await _jobStore.Save(null);
        }

        [TestMethod]
        public async Task Save_Concurrent_Should_Save()
        {
            var tasks = Enumerable.Range(0, 50)
                .Select(x => Task.Run(async () => await _jobStore.Save(new Job())))
                .ToList();

            var savedJobs = await Task.WhenAll(tasks);

            foreach (var savedJob in savedJobs)
            {
                Assert.AreEqual(savedJob, await _jobStore.Get(savedJob.Id));
            }
        }

        [TestMethod]
        public async Task Get_When_Valid_Job_Should_Return()
        {
            var savedJob = await _jobStore.Save(new Job());

            Assert.AreEqual(savedJob, await _jobStore.Get(savedJob.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Get_When_Null_Should_Throw_ArgumentNullException()
        {
            await _jobStore.Get(null);
        }

        [TestMethod]
        public async Task Get_Concurrent_Should_Get()
        {
            var savedJobs = Enumerable.Range(0, 50)
                .Select(async x => await _jobStore.Save(new Job()))
                .Select(t=> t.Result)
                .ToList();

            var getTasks = savedJobs
                .Select(job => Task.Run(async () => await _jobStore.Get(job.Id)))
                .ToList();
            var getJobs = await Task.WhenAll(getTasks);

            CollectionAssert.AreEqual(savedJobs, getJobs);
        }
    }
}
