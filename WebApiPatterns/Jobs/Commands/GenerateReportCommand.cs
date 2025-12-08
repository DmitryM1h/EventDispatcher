using WebApiPatterns.Interfaces;

namespace WebApiPatterns.Jobs.Commands
{
    public class GenerateReportCommand : CommandBase
    {
        public string Description { get; init; } = null!;

        public GenerateReportCommand(string description)
        {
            Description = description;
        }
    }
}
