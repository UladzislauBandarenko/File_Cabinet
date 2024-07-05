namespace FileCabinetApp;

public class FileCabinetMemoryIterator : IFileCabinetRecordIterator
{
    private readonly List<FileCabinetRecord> records;
    private int currentIndex = -1;

    public FileCabinetMemoryIterator(List<FileCabinetRecord> records)
    {
        this.records = records;
    }

    public FileCabinetRecord Current => this.currentIndex >= 0 && this.currentIndex < this.records.Count ? this.records[this.currentIndex] : null;

    public bool MoveNext()
    {
        if (this.currentIndex < this.records.Count - 1)
        {
            this.currentIndex++;
            return true;
        }

        return false;
    }

    public void Reset()
    {
        this.currentIndex = -1;
    }
}