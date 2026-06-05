using WebApiPatterns.Jobs;

namespace WebApiPatterns.Interfaces
{
    public class CommandBase
    {
        public Initiator initiator { get; init; } = null!;

    }
}
