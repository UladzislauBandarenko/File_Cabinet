namespace FileCabinetApp.CommandHandlers
{
    public interface ICommandHandler
    {
        void Handle(string command);

        void SetNext(ICommandHandler handler);
    }
}