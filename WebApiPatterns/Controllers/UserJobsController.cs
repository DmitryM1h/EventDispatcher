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
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize]
        public async Task<ActionResult> ExportDataToExternalSystem([FromBody] string description)
        {
            var command = new ExportDataCommand(description);

            await _jobMediator.ReceiveCommand(command);

            return Accepted();
        }


        [HttpPost("GenerateReport")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize]
        public async Task<ActionResult> GenerateReport([FromBody] string description)
        {
            var command = new GenerateReportCommand();

            await _jobMediator.ReceiveCommand(command);

            return Accepted();
        }

    }
}
