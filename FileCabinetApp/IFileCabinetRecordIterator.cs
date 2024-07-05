namespace FileCabinetApp;

public interface IFileCabinetRecordIterator : IEnumerator<FileCabinetRecord>
{
    bool MoveNext();

    FileCabinetRecord Current { get; }

    void Reset();
}
