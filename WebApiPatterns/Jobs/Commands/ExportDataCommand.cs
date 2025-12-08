using WebApiPatterns.Interfaces;

namespace WebApiPatterns.Jobs.Commands
{
    public class ExportDataCommand : CommandBase
    {
        public string Description { get; init; } = null!;

        public ExportDataCommand(string username, string description)
        {
            base.UserName = username;
            Description = description;
        }
    }
}
