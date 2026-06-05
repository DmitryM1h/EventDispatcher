using WebApiPatterns.Interfaces;
using WebApiPatterns.Jobs.Commands;

namespace WebApiPatterns.Jobs
{
    public class ExportDataToExternalSystem : JobHandlerBase<ExportDataCommand>, IJobHandler<ExportDataCommand>
    {
        public ExportDataToExternalSystem(IServiceScopeFactory serviceScopeFactory, Initiator initiator) : base(serviceScopeFactory, initiator) { }

        protected override async IAsyncEnumerable<ProgressBar> ExecuteJobAsync(ExportDataCommand command)
        {

            Task.Delay(TimeSpan.FromSeconds(5)).Wait(); // имитация синхронной работы

            yield return new ProgressBar(25);

            Task.Delay(TimeSpan.FromSeconds(5)).Wait();

            yield return new ProgressBar(50);

            Task.Delay(TimeSpan.FromSeconds(5)).Wait();

            yield return new ProgressBar(75);

            Task.Delay(TimeSpan.FromSeconds(5)).Wait();

            yield return new ProgressBar(100);

            Console.WriteLine("Export data completed");
        }


     


        //[Obsolete]
        //private async Task ExecuteJobObsolete(ExportDataCommand command)
        //{

        //    await NotifyProgress();

        //    await Task.Delay(TimeSpan.FromSeconds(10));
        //    ProgressPercent = 50;
        //    await NotifyProgress();

        //    ThrowIfTaskCancelled();

        //    await Task.Delay(TimeSpan.FromSeconds(5));
        //    ProgressPercent = 75;
        //    await NotifyProgress();

        //    ThrowIfTaskCancelled();

        //    await Task.Delay(TimeSpan.FromSeconds(5));
        //    ProgressPercent = 100;
        //    await NotifyProgress();

        //    Console.WriteLine("Export data completed");
        //}
    }
}
