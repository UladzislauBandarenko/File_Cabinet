namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command handler base.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        /// <summary>
        /// The next handler.
        /// </summary>
        protected ICommandHandler? nextHandler;

        /// <inheritdoc/>
        public void SetNext(ICommandHandler handler)
        {
            this.nextHandler = handler;
        }

        /// <inheritdoc/>
        public abstract void Handle(string command);
    }
}