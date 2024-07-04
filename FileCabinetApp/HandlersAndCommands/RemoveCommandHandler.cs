namespace FileCabinetApp.CommandHandlers
{
    public class RemoveCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        public RemoveCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
        }

        public override void Handle(string command)
        {
            if (command.StartsWith("remove", StringComparison.InvariantCultureIgnoreCase))
            {
                var inputs = command.Split(' ', 2);
                if (inputs.Length < 2 || !int.TryParse(inputs[1], out int id))
                {
                    Console.WriteLine($"Invalid record id: {inputs[1]}");
                    return;
                }

                try
                {
                    if (fileCabinetService.RemoveRecord(id))
                    {
                        Console.WriteLine($"Record #{id} is removed.");
                    }
                    else
                    {
                        Console.WriteLine($"Record #{id} does not exist.");
                    }
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Failed to remove record: {ex.Message}");
                }
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(command);
            }
        }
    }
}