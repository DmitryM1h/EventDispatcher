using WebApiPatterns.Interfaces;
using WebApiPatterns.Jobs.Commands;

namespace WebApiPatterns.Jobs
{
    public class GenerateReport : JobHandlerBase<GenerateReportCommand>, IJobHandler<GenerateReportCommand>
    {
        public GenerateReport(IServiceProvider serviceProvider, string initiator) : base(serviceProvider, initiator) { }
       
        protected override IAsyncEnumerable<int> ExecuteJobAsync(GenerateReportCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
