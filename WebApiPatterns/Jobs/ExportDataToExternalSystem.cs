using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using WebApiPatterns.Application;
using WebApiPatterns.Interfaces;
using WebApiPatterns.Jobs.Commands;

namespace WebApiPatterns.Jobs
{
    public class ExportDataToExternalSystem : IJobHandler<ExportDataCommand>
    {
        private string Initiator { get; init; } = null!;
        private int ProgressPercent { get; set; }

        private readonly IHubContext<NotificationHub> HubContext;

        private static ConcurrentDictionary<string, CancellationTokenSource> activeTasks = new();

        public static CancellationTokenSource GetUsersToken(string inititor) => activeTasks[inititor];

        public void ThrowIfTaskCancelled()
        {
            activeTasks[Initiator].Token.ThrowIfCancellationRequested();
        }

        public ExportDataToExternalSystem(IServiceProvider serviceProvider)
        {
            Initiator = "TestUser";
            ProgressPercent = 0;
            HubContext = serviceProvider.GetRequiredService<IHubContext<NotificationHub>>();

            var src = new CancellationTokenSource();

            src.Token.Register(async () => await NotifyCancel());

            activeTasks[Initiator] = src;
        }
        public async Task ExecuteJob(ExportDataCommand command)
        {
            // Если один пользователь уже запустил эту задачу, то не запускать еще раз?
            // Добавить возможность отменить задачу
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

        private async Task NotifyProgress()
        {
            await HubContext.Clients.All.SendAsync("ExportDataTaskProgress", new {Initiator, ProgressPercent});
        }
        private async Task NotifyCancel()
        {
            await HubContext.Clients.All.SendAsync("ExportDataTaskProgress", "Задача отменена пользователем");
        }

    }
}
