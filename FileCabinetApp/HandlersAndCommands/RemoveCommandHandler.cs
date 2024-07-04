namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Remove command handler.
    /// </summary>
    public class RemoveCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        public RemoveCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
        }

        /// <inheritdoc/>
        public override void Handle(string command)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command.StartsWith("remove", StringComparison.OrdinalIgnoreCase))
            {
                var inputs = command.Split(' ', 2);
                if (inputs.Length < 2 || !int.TryParse(inputs[1], out int id))
                {
                    Console.WriteLine($"Invalid record id: {inputs[1]}");
                    return;
                }

                try
                {
                    if (this.fileCabinetService.RemoveRecord(id))
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