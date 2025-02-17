using FileCabinetApp.Models;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Export command handler.
    /// </summary>
    public class ExportCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;
        private readonly IReadOnlyCollection<HelpMessage> helpMessages;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        /// <param name="helpMessages">The help messages.</param>
        public ExportCommandHandler(IFileCabinetService fileCabinetService, IReadOnlyCollection<HelpMessage> helpMessages)
        {
            this.fileCabinetService = fileCabinetService;
            this.helpMessages = helpMessages;
        }

        /// <inheritdoc/>
        public override void Handle(string command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command.StartsWith("export", StringComparison.InvariantCultureIgnoreCase))
            {
                var inputs = command.Split(' ', 3);
                if (inputs.Length < 3)
                {
                    var exportHelp = this.helpMessages.FirstOrDefault(m => m.Command == "export");
                    Console.WriteLine("Invalid command format.");
                    if (exportHelp != null)
                    {
                        Console.WriteLine(exportHelp.DetailedDescription);
                    }

                    return;
                }

                var format = inputs[1].ToLowerInvariant();
                var fileName = inputs[2];

                try
                {
                    var snapshot = this.fileCabinetService.MakeSnapshot();
                    if (File.Exists(fileName))
                    {
                        Console.Write($"File is exist - rewrite {fileName}? [Y/n] ");
                        var answer = Console.ReadLine();
                        if (string.IsNullOrEmpty(answer) || answer.ToUpperInvariant() != "Y")
                        {
                            Console.WriteLine("Export canceled.");
                            return;
                        }
                    }

                    using (var writer = new StreamWriter(fileName))
                    {
                        switch (format)
                        {
                            case "csv":
                                var csvWriter = new FileCabinetRecordCsvWriter(writer);
                                csvWriter.Write(snapshot.Records);
                                break;
                            case "xml":
                                var xmlWriter = new FileCabinetRecordXmlWriter(writer);
                                xmlWriter.Write(snapshot.Records);
                                break;
                            default:
                                Console.WriteLine($"Unsupported export format: {format}");
                                return;
                        }
                    }

                    Console.WriteLine($"All records are exported to file {fileName}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Export failed: {ex.Message}");
                }
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(command);
            }
        }
    }
}