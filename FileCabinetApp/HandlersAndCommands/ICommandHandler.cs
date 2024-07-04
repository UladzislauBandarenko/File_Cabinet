namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command handler.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Handles the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        void Handle(string command);

        /// <summary>
        /// Sets the next handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        void SetNext(ICommandHandler handler);
    }
}