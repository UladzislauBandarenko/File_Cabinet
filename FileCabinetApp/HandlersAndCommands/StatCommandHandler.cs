namespace FileCabinetApp.CommandHandlers
{
    public class StatCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        public StatCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
        }

        public override void Handle(string command)
        {
            if (command.Equals("stat", StringComparison.InvariantCultureIgnoreCase))
            {
                var recordsCount = this.fileCabinetService.GetStat();
                Console.WriteLine($"{recordsCount} record(s).");
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(command);
            }
        }
    }
}
