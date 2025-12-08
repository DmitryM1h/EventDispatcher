using WebApiPatterns.Interfaces;
using WebApiPatterns.Jobs.Commands;

namespace WebApiPatterns.Jobs
{
    public class ExportDataToExternalSystem : JobHandlerBase<ExportDataCommand>, IJobHandler<ExportDataCommand>
    {
        public ExportDataToExternalSystem(IServiceProvider serviceProvider, string initiator) : base(serviceProvider, initiator) { }

        protected override async IAsyncEnumerable<int> ExecuteJobAsync(ExportDataCommand command)
        {

            await Task.Delay(TimeSpan.FromSeconds(5));
            ProgressPercent = 25;

            yield return 1;

            await Task.Delay(TimeSpan.FromSeconds(5));
            ProgressPercent = 50;

            yield return 1;

            await Task.Delay(TimeSpan.FromSeconds(5));
            ProgressPercent = 75;

            yield return 1;

            await Task.Delay(TimeSpan.FromSeconds(5));
            ProgressPercent = 100;

            yield return 1;

            Console.WriteLine("Export data completed");
        }


     


        [Obsolete]
        private async Task ExecuteJobObsolete(ExportDataCommand command)
        {

            await NotifyProgress();

            await Task.Delay(TimeSpan.FromSeconds(10));
            ProgressPercent = 50;
            await NotifyProgress();

            ThrowIfTaskCancelled();

            await Task.Delay(TimeSpan.FromSeconds(5));
            ProgressPercent = 75;
            await NotifyProgress();

            ThrowIfTaskCancelled();

            await Task.Delay(TimeSpan.FromSeconds(5));
            ProgressPercent = 100;
            await NotifyProgress();

            Console.WriteLine("Export data completed");
        }
    }
}
