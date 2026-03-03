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

        private JobCompletionReporter _progressReporter;
        
        private static ConcurrentDictionary<string, CancellationTokenSource> activeTasks = new();

        ILogger<JobHandlerBase<ICommand>> _logger;

        protected JobHandlerBase(IServiceProvider serviceProvider, string initiator)
        {
            Initiator = initiator;
            ProgressPercent = 0;

            _logger = serviceProvider.GetRequiredService<ILogger<JobHandlerBase<ICommand>>>();

            _progressReporter = serviceProvider.GetRequiredService<JobCompletionReporter>();

            var src = new CancellationTokenSource();

            activeTasks[initiator] = src;

        }

        public async Task ExecuteJob(ICommand command)
        {
            try
            {
                await foreach (var _ in ExecuteJobAsync(command))
                {
                    await _progressReporter.NotifyProgress(Initiator, ProgressPercent);

                    await Task.Yield();

                    ThrowIfTaskCancelled();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Job jas been cancelled by {user}", Initiator);
                await _progressReporter.NotifyCancel(Initiator);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Application job failed {details}", ex.ToString());
                await _progressReporter.NotifyError(Initiator);
            }
            finally
            {
                activeTasks[Initiator].Dispose();
            }
        }

        public static void CancelTask(string initiator)
        {
            activeTasks[initiator].Cancel();
        }

        protected void ThrowIfTaskCancelled()
        {
            activeTasks[Initiator].Token.ThrowIfCancellationRequested();
        }
        protected abstract IAsyncEnumerable<int> ExecuteJobAsync(ICommand command);
       
    }


    //todo generic
    public class JobCompletionReporter(IHubContext<NotificationHub> HubContext)
    {
        public async Task NotifyProgress(string Initiator, int ProgressPercent)
        {
            await HubContext.Clients.All.SendAsync("ExportDataTaskProgress", new { Initiator, ProgressPercent });
        }
        public async Task NotifyCancel(string Initiator)
        {
            await HubContext.Clients.All.SendAsync("ExportDataTaskCancelled", $"Задача отменена пользователем {Initiator}");
        }
        public async Task NotifyError(string Initiator)
        {
            await HubContext.Clients.All.SendAsync("ExportDataTaskReceiveError", $"Задача завершена с ошибкой. Инциатор: {Initiator}");
        } 
    }

}
