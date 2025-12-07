using Microsoft.AspNetCore.Mvc;
using WebApiPatterns.Application;
using WebApiPatterns.Jobs.Commands;

namespace WebApiPatterns.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UserJobsController(JobMediator _jobMediator) : ControllerBase
    {

        [HttpPost("ExportData")]
        //[Authorize]
        public async Task<ActionResult> ExportDataToExternalSystem([FromBody] string description)
        {
            var command = new ExportDataCommand(description);

            await _jobMediator.ReceiveCommand(command);

            return Accepted();
        }


    }
}
