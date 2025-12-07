using System.Data;
using System.Reflection;
using WebApiPatterns.Interfaces;

namespace WebApiPatterns.Application
{
    public abstract class JobBase
    {

    }
    public class JobMediator
    {
        public async Task ReceiveCommand(CommandBase command)
        {
            var type = command.GetType();

            var handler = Assembly.GetExecutingAssembly()
                        .DefinedTypes
                        .Where(t => t.IsClass)
                        .Where(t =>
                            t.ImplementedInterfaces
                            .Any(x => x.Name == typeof(IJobHandler<>).Name && x.GenericTypeArguments.Contains(type))
                         )
                        .FirstOrDefault() ?? throw new ArgumentOutOfRangeException($"Unable to resolve for command {type.Name}");

            var handlertype = handler.AsType();

            var interfaceType = typeof(IJobHandler<>).MakeGenericType(type);

            var handlerInstance = Activator.CreateInstance(handlertype);

            var method = interfaceType.GetMethod("ExecuteJob");

            var task = method!.Invoke(handlerInstance, null) as Task;
        }
    }
}
