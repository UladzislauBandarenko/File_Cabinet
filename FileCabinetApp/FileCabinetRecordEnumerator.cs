namespace FileCabinetApp;

using System.Collections;

public class FileCabinetRecordEnumerator : IEnumerator<FileCabinetRecord>
{
    private readonly IFileCabinetService service;
    private readonly IEnumerator<FileCabinetRecord> enumerator;

    public FileCabinetRecordEnumerator(IFileCabinetService service)
    {
        this.service = service;
        this.enumerator = service.GetRecords(r => r.ToString()).GetEnumerator();
    }

    public FileCabinetRecord Current => enumerator.Current;

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        enumerator.Dispose();
    }

    public bool MoveNext()
    {
        return enumerator.MoveNext();
    }

    public void Reset()
    {
        enumerator.Reset();
    }
}