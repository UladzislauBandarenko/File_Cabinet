namespace FileCabinetApp.CommandHandlers
{
    public class HelpCommandHandler : CommandHandlerBase
    {
        private readonly string[][] helpMessages;

        public HelpCommandHandler(string[][] helpMessages)
        {
            this.helpMessages = helpMessages;
        }

        public override void Handle(string command)
        {
            if (command.StartsWith("help", StringComparison.InvariantCultureIgnoreCase))
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
                var index = Array.FindIndex(this.helpMessages, 0, this.helpMessages.Length, i => string.Equals(i[0], parameters, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(this.helpMessages[index][2]);
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
                    Console.WriteLine($"\t{helpMessage[0]}\t- {helpMessage[1]}");
                }
            }

            Console.WriteLine();
        }
    }
}