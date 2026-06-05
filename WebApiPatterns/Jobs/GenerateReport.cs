using WebApiPatterns.Interfaces;
using WebApiPatterns.Jobs.Commands;

namespace WebApiPatterns.Jobs
{
    public class GenerateReport : JobHandlerBase<GenerateReportCommand>, IJobHandler<GenerateReportCommand>
    {
        public GenerateReport(IServiceScopeFactory serviceScopeFactory, Initiator initiator) : base(serviceScopeFactory, initiator) { }
       
        protected override IAsyncEnumerable<ProgressBar> ExecuteJobAsync(GenerateReportCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
