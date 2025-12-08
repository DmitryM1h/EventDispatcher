using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using WebApiPatterns.Application;
using WebApiPatterns.Interfaces;

namespace WebApiPatterns.Jobs
{
    public abstract class JobHandlerBase<ICommand> where ICommand : CommandBase
    {
        private string Initiator { get; set; } = null!;
        protected int ProgressPercent { get; set; }

        private readonly IHubContext<NotificationHub> HubContext;

        private static ConcurrentDictionary<string, CancellationTokenSource> activeTasks = new();

        protected JobHandlerBase(IServiceProvider serviceProvider, string initiator)
        {
            Initiator = initiator;
            ProgressPercent = 0;
            HubContext = serviceProvider.GetRequiredService<IHubContext<NotificationHub>>();

            var src = new CancellationTokenSource();

            src.Token.Register(async () => await NotifyCancel());

            activeTasks[initiator] = src;

        }


        public async Task ExecuteJob(ICommand command)
        {
            await foreach (var _ in ExecuteJobAsync(command))
            {
                await NotifyProgress();

                await Task.Delay(20);

                ThrowIfTaskCancelled();
            }

            activeTasks[Initiator].Dispose();
        }

        protected abstract IAsyncEnumerable<int> ExecuteJobAsync(ICommand command);

        protected async Task NotifyProgress()
        {
            await HubContext.Clients.All.SendAsync("ExportDataTaskProgress", new { Initiator, ProgressPercent });
        }
        protected async Task NotifyCancel()
        {
            await HubContext.Clients.All.SendAsync("ExportDataTaskProgress", "Задача отменена пользователем");
        }


        public static void CancelTask(string initiator)
        {
            activeTasks[initiator].Cancel();

            activeTasks[initiator].Dispose();
        }

        protected void ThrowIfTaskCancelled()
        {
            activeTasks[Initiator].Token.ThrowIfCancellationRequested();
        }
    }
}
