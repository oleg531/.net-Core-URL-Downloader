namespace Downloader.API.Controllers
{
    using System.Threading.Tasks;
    using Core;
    using Core.Exceptions;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    public class JobsController : Controller
    {
        private readonly IMediator _mediator;

        public JobsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]JobAddContext job)
        {
            if (job == null)
            {
                return BadRequest("Job is empty or malformed.");
            }

            var savedJob = await _mediator.Send(job);
            return Ok(savedJob);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Missing parameter id.");
            }

            var jobQueryContext = new JobQueryContext(id);

            try
            {
                return Ok(await _mediator.Send(jobQueryContext));
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
