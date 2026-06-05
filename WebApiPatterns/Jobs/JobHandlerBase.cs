using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using WebApiPatterns.Application;
using WebApiPatterns.Interfaces;

namespace WebApiPatterns.Jobs
{
    public record ProgressBar(int percentage);
    public record Initiator(string name);

    public abstract class JobHandlerBase<ICommand> : IAsyncDisposable where ICommand : CommandBase
    {
        private Initiator Initiator { get; set; } = null!;

        private JobCompletionReporter _progressReporter;
        
        private static readonly ConcurrentDictionary<Initiator, CancellationTokenSource> activeTasks = new();

        private AsyncServiceScope serviceScope;

        ILogger<JobHandlerBase<ICommand>> _logger;


        /// <summary>
        ///  Т.к. используется мой простой рукописный медиатор, он не умеет делать DI, поэтому просто везде пихаю в параметры serviceProvider
        /// </summary>
        protected JobHandlerBase(IServiceScopeFactory scopeFactory, Initiator initiator)
        {
            Initiator = initiator;
            serviceScope = scopeFactory.CreateAsyncScope();

            _logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<JobHandlerBase<ICommand>>>();

            _progressReporter = serviceScope.ServiceProvider.GetRequiredService<JobCompletionReporter>();

            var src = new CancellationTokenSource();

            activeTasks[initiator] = src;

        }

        protected abstract IAsyncEnumerable<ProgressBar> ExecuteJobAsync(ICommand command);

        public async Task ExecuteJob(ICommand command)
        {
            try
            {
                await foreach (var progressBar in ExecuteJobAsync(command))
                {
                    await _progressReporter.NotifyProgress(Initiator, progressBar);

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

        public static void CancelTask(Initiator initiator)
        {
            activeTasks[initiator].Cancel();
            activeTasks[initiator].Dispose();
        }

        protected void ThrowIfTaskCancelled()
        {
            activeTasks[Initiator].Token.ThrowIfCancellationRequested();
        }

        public async ValueTask DisposeAsync()
        {
            await serviceScope.DisposeAsync();

            foreach (var task in activeTasks.Values)
            {
                task.Cancel();
                task.Dispose();
            }
        }
    }


    //todo generic
    public class JobCompletionReporter(IHubContext<NotificationHub> HubContext)
    {
        public async Task NotifyProgress(Initiator Initiator, ProgressBar progressBar)
        {
            await HubContext.Clients.All.SendAsync("ExportDataTaskProgress", new { Initiator, progressBar.percentage });
        }
        public async Task NotifyCancel(Initiator Initiator)
        {
            await HubContext.Clients.All.SendAsync("ExportDataTaskCancelled", $"Задача отменена пользователем {Initiator}");
        }
        public async Task NotifyError(Initiator Initiator)
        {
            await HubContext.Clients.All.SendAsync("ExportDataTaskReceiveError", $"Задача завершена с ошибкой. Инциатор: {Initiator}");
        } 
    }

}
