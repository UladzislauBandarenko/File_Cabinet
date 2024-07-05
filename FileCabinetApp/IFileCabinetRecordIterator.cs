namespace FileCabinetApp;

public interface IFileCabinetRecordIterator
{
    bool MoveNext();

    FileCabinetRecord Current { get; }

    void Reset();
}