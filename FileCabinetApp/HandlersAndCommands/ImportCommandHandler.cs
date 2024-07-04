namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Import command handler.
    /// </summary>
    public class ImportCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        public ImportCommandHandler(IFileCabinetService fileCabinetService)
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

            if (command.StartsWith("import", StringComparison.InvariantCultureIgnoreCase))
            {
                var inputs = command.Split(' ', 3);
                if (inputs.Length < 3)
                {
                    Console.WriteLine("Invalid command format. Usage: import [csv|xml] <filename>");
                    return;
                }

                var format = inputs[1].ToLowerInvariant();
                var fileName = inputs[2];

                if (!File.Exists(fileName))
                {
                    Console.WriteLine($"File {fileName} does not exist.");
                    return;
                }

                try
                {
                    FileCabinetServiceSnapshot snapshot;
                    switch (format)
                    {
                        case "csv":
                            using (var reader = new StreamReader(fileName))
                            {
                                var csvReader = new FileCabinetRecordCsvReader(reader);
                                snapshot = csvReader.ReadAll();
                            }

                            break;
                        case "xml":
                            using (var reader = new StreamReader(fileName))
                            {
                                var xmlReader = new FileCabinetRecordXmlReader(reader);
                                snapshot = xmlReader.ReadAll();
                            }

                            break;
                        default:
                            Console.WriteLine($"Unsupported import format: {format}");
                            return;
                    }

                    this.fileCabinetService.Restore(snapshot);
                    Console.WriteLine($"All records from {fileName} were imported.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while importing: {ex.Message}");
                }
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(command);
            }
        }
    }
}