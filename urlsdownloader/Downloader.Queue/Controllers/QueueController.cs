namespace Downloader.Queue.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Core.Queue;
    using MediatR;

    [Route("api/[controller]")]
    public class QueueController : Controller
    {
        private readonly IMediator _mediator;

        public QueueController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("enqueue/{id}")]
        public async Task<IActionResult> Enqueue(string id)
        {
            var context = new EnqueueJobContext
            {
                Id = id
            };
            await _mediator.Send(context);

            return Ok();
        }

        [HttpPost("dequeue")]
        public async Task<IActionResult> Dequeue()
        {
            var context = new DequeueJobContext();
            var jobTask = await _mediator.Send(context);
            return Ok(jobTask);
        }
    }
}
