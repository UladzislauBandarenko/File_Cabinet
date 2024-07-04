namespace FileCabinetApp.CommandHandlers
{
    public class PurgeCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        public PurgeCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
        }

        public override void Handle(string command)
        {
            if (command.Equals("purge", StringComparison.InvariantCultureIgnoreCase))
            {
                if (fileCabinetService is FileCabinetFilesystemService filesystemService)
                {
                    int totalRecords = filesystemService.GetStat();
                    int purgedRecords = filesystemService.PurgeRecords();
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
