namespace FileCabinetApp.CommandHandlers
{
    public class ExitCommandHandler : CommandHandlerBase
    {
        private readonly Action<bool> setIsRunning;

        public ExitCommandHandler(Action<bool> setIsRunning)
        {
            this.setIsRunning = setIsRunning;
        }

        public override void Handle(string command)
        {
            if (command.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Exiting an application...");
                this.setIsRunning(false);
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(command);
            }
        }
    }
}
