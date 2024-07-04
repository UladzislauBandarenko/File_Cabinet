namespace FileCabinetApp.CommandHandlers
{
    public abstract class CommandHandlerBase : ICommandHandler
    {
        protected ICommandHandler nextHandler;

        public void SetNext(ICommandHandler handler)
        {
            this.nextHandler = handler;
        }

        public abstract void Handle(string command);
    }
}