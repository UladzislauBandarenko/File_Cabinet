using FileCabinetApp.HandlersAndCommands;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Help command handler.
    /// </summary>
    public class HelpCommandHandler : CommandHandlerBase
    {
        private readonly List<HelpMessage> helpMessages;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpCommandHandler"/> class.
        /// </summary>
        /// <param name="helpMessages">The help messages.</param>
        public HelpCommandHandler(IReadOnlyCollection<HelpMessage> helpMessages)
        {
            this.helpMessages = helpMessages.ToList();
        }

        /// <inheritdoc/>
        public override void Handle(string command)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command.StartsWith("help", StringComparison.OrdinalIgnoreCase))
            {
                string[] parts = command.Split(' ', 2);
                string parameters = parts.Length > 1 ? parts[1] : string.Empty;
                this.PrintHelp(parameters);
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(command);
            }
        }

        private void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var helpMessage = this.helpMessages.Find(h => string.Equals(h.Command, parameters, StringComparison.OrdinalIgnoreCase));
                if (helpMessage != null)
                {
                    Console.WriteLine(helpMessage.DetailedDescription);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in this.helpMessages)
                {
                    Console.WriteLine($"\t{helpMessage.Command}\t- {helpMessage.ShortDescription}");
                }
            }

            Console.WriteLine();
        }
    }
}