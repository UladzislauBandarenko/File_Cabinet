namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Purge command handler.
    /// </summary>
    public class PurgeCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        public PurgeCommandHandler(IFileCabinetService fileCabinetService)
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

            if (command.Equals("purge", StringComparison.OrdinalIgnoreCase))
            {
                var service = this.fileCabinetService;
                while (service is ServiceMeter serviceMeter)
                {
                    service = serviceMeter.Service;
                }

                if (service is FileCabinetFilesystemService)
                {
                    int totalRecords = this.fileCabinetService.GetStat();
                    int purgedRecords = this.fileCabinetService.PurgeRecords();
                    Console.WriteLine($"Data file processing is completed: {purgedRecords} of {totalRecords} records were purged.");
                }
                else
                {
                    Console.WriteLine("Purge command is available only for file storage.");
                }
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(command);
            }
        }
    }
}
