using System.Data;
using System.Reflection;
using WebApiPatterns.Exceptions;
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

            var handlers = Assembly.GetExecutingAssembly()
                        .DefinedTypes
                        .Where(t => t.IsClass)
                        .Where(t =>
                            t.ImplementedInterfaces
                            .Any(x => x.Name == typeof(IJobHandler<>).Name && x.GenericTypeArguments.Contains(type))
                         ).ToList();

            if(handlers.Count == 0)
                throw new HandlerNotFoundException($"Unable to resolve for command {type.Name}");

            if (handlers.Count > 1)
                throw new MultipleHandlersException($"Multiple handlers for command {type.Name}");

            var handler = handlers.First();

            var handlertype = handler.AsType();

            var interfaceType = typeof(IJobHandler<>).MakeGenericType(type);

            var handlerInstance = Activator.CreateInstance(handlertype);

            var method = interfaceType.GetMethod("ExecuteJob");

            method!.Invoke(handlerInstance, null);
        }
    }
}
