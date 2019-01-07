namespace Downloader.Core.Tests.Queue
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Queue;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JobQueueTests
    {
        private readonly IJobQueue _jobQueue;

        public JobQueueTests()
        {
            _jobQueue = new JobQueue();
        }

        [TestMethod]
        public async Task Enqueue_When_JobTask_Present_Should_Add()
        {
            var jobTask = new JobTask
            {
                Id = Guid.NewGuid().ToString()
            };
            await _jobQueue.Enqueue(jobTask);

            var expected = await _jobQueue.Dequeue();

            Assert.AreEqual(jobTask,expected);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Enqueue_Null_Should_Throw_ArgumentNullException()
        {
            await _jobQueue.Enqueue(null);
        }

        [TestMethod]
        public async Task Enqueue_Concurrent_Should_Add()
        {
            var jobTasks = Enumerable.Range(0, 50)
                .Select(x => new JobTask
                {
                    Id = Guid.NewGuid().ToString()
                })
                .ToList();

            var enqTasks = jobTasks
                .Select(jobTask => Task.Run(async () => await _jobQueue.Enqueue(jobTask)))
                .ToList();

            await Task.WhenAll(enqTasks);

            var deqJobTasks =  Enumerable.Range(0, 50)
                .Select(async x => await _jobQueue.Dequeue())
                .Select(t => t.Result)
                .ToList();

            foreach (var deqJobTask in deqJobTasks)
            {
                Assert.IsTrue(jobTasks.Any(x=>x.Id == deqJobTask.Id));
            }

        }

        [TestMethod]
        public async Task Dequeue_When_Task_Present_Should_Dequeue()
        {
            var jobTask = new JobTask
            {
                Id = Guid.NewGuid().ToString()
            };
            await _jobQueue.Enqueue(jobTask);

            var expected = await _jobQueue.Dequeue();

            Assert.AreEqual(jobTask, expected);
        }

        [TestMethod]
        public async Task Dequeue_When_Empty_Should_Return_Null()
        {
            Assert.IsNull(await _jobQueue.Dequeue());
        }

        [TestMethod]
        public async Task Dequeue_Concurrent_Should_Dequeue()
        {
            var jobTasks = Enumerable.Range(0, 50)
                .Select(x => new JobTask
                {
                    Id = Guid.NewGuid().ToString()
                })
                .ToList();

            jobTasks.ForEach(async jobTask => await _jobQueue.Enqueue(jobTask));


            var tasks = Enumerable.Range(0, 50)
                .Select(x => Task.Run(async () => await _jobQueue.Dequeue()))
                .ToList();
            var deqJobTasks = await Task.WhenAll(tasks);


            foreach (var deqJobTask in deqJobTasks)
            {
                Assert.IsTrue(jobTasks.Any(x => x.Id == deqJobTask.Id));
            }

        }
    }
}
